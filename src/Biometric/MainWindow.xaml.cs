using Biometric.Algorithms;
using Biometric.Controller;
using Biometric.Models;
using Biometric.Repository;
using DotNetEnv;
using Microsoft.Win32;
using MySqlConnector;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Diagnostics;

namespace Biometric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private BitmapImage sidikJari = null;
        private BitmapImage sidikJariHasil = null;
        private string algorithm = "BM";
        private Processor p = new Processor();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Use an asynchronous method to perform initialization
        private async Task InitializeAsync()
        {
            MessageBox.Show(Directory.GetCurrentDirectory());

            Env.Load("..\\..\\.env");

            string server = Env.GetString("DB_SERVER");
            string user = Env.GetString("DB_USER");
            string password = Env.GetString("DB_PASS");
            string database = Env.GetString("DB_DATABASE");

            string connectionString = $"Server={server};User={user};Password={password};Database={database};";

            MessageBox.Show(connectionString);

            // Execute the query using Dapper
            using (var connection = new MySqlConnection(connectionString))
            {
                // Step 1: Create an instance of PersonRepository
                PersonRepository pr = new PersonRepository(connectionString);

                // Step 2: Create a new Person object with the required details
                Person test = new Person
                {
                    NIK = "13522080",  // Assuming this is the NIK
                    nama = "John Doe",
                    tempat_lahir= "Jakarta",
                    tanggal_lahir = new DateTime(1990, 1, 1), // Example date of birth
                    jenis_kelamin = "Laki-Laki",
                    golongan_darah = "O",
                    alamat = "Jl. Sudirman No. 10",
                    agama = "Islam",
                    status_perkawinan = "Belum Menikah",
                    pekerjaan = "Developer",
                    kewarganegaraan = "Indonesia",
                };
                MessageBox.Show(test.ToString());

                // Step 3: Call InsertPersonAsync method to insert the new person
                int rowsAffected = await pr.InsertPersonAsync(test);

                MessageBox.Show(rowsAffected.ToString());
                IEnumerable<Person> allPersons = await pr.GetAllPersonsAsync();

                // Convert rowsAffected to string before showing it in MessageBox
                MessageBox.Show(((allPersons.First()).NIK).ToString());
            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Load the selected image file into the image control
                    sidikJari = new BitmapImage(new Uri(openFileDialog.FileName));
                    Console.WriteLine(openFileDialog.FileName);
                    p.setAttributes(openFileDialog.FileName);
                    imageControl.Source = sidikJari;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        private string showPerson(Person p)
        {
            string ret = "";
            ret += "NIK: " + p.NIK + "\n";
            ret += "Nama: " + p.nama + "\n";
            ret += "Tempat lahir: " + p.tempat_lahir + "\n";
            DateTime dt = p.tanggal_lahir.GetValueOrDefault();
            ret += "Tanggal lahir: " + dt.Day + "-" + dt.Month + "-" + dt.Year + "\n";
            ret += "Jenis Kelamin: " + p.jenis_kelamin+ "\n";
            ret += "Alamat: " + p.alamat+ "\n";
            ret += "Agama: " + p.agama + "\n";
            ret += "Status Perkawinan: " + p.status_perkawinan+ "\n";
            ret += "Pekerjaan: " + p.pekerjaan + "\n";
            ret += "Kewarganegaraan: " + p.kewarganegaraan;

            return ret;
        }

        public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // Convert BitmapImage to a MemoryStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Create a Bitmap from the MemoryStream
                Bitmap bitmap = new Bitmap(memoryStream);
                return bitmap;
            }
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            if (sidikJari == null)
            {
                MessageBox.Show("Mohon pilih sebuah gambar");
            }
            else
            {
                Console.WriteLine("starting");
                Env.Load("..\\..\\.env");
                string server = Env.GetString("DB_SERVER");
                string user = Env.GetString("DB_USER");
                string password = Env.GetString("DB_PASS");
                string database = Env.GetString("DB_DATABASE");

                string connectionString = $"Server={server};User={user};Password={password};Database={database};";
                Console.WriteLine("connection to db established");

                List<Fingerprint> fingerprints;
                Fingerprint res = new Fingerprint();
                using (var connection = new MySqlConnection(connectionString))
                {
                    FingerprintRepository fr = new FingerprintRepository(connectionString);
                    var fingerprintTask = fr.GetAllFingerprintAsync(); // Get the task
                    var fingerprintsResult = await fingerprintTask; // Await the task to get the result
                    fingerprints = fingerprintsResult.ToList(); // Convert the result to a list
                }

                Console.WriteLine($"panjang fingerprints: {fingerprints.Count()}");
                Console.WriteLine("fingerprint loaded");
                double similarity = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (radioButton2.IsChecked == true) 
                {
                    algorithm = "KMP";
                    Console.WriteLine("KMP");
                    var kmpResult = await Task.Run(() => parallelKmpComparator(fingerprints));

                    if (kmpResult.Item1)
                    {
                        res = kmpResult.Item2;
                        similarity = 1;
                    }
                    else
                    {
                        var levResult = await Task.Run(() => minLevenshteinComparator(fingerprints));
                        res = levResult.Item2;
                        similarity = levResult.Item1;
                    }
                }
                else
                {
                    algorithm = "BM";
                    Console.WriteLine("BM");
                    var bmResult = manualBmComparator(fingerprints);

                    if (bmResult.Item1)
                    {
                        Console.WriteLine("found");
                        res = bmResult.Item2;
                        similarity = 1;
                    }
                    else
                    {
                        Console.WriteLine("not found");
                        var levResult = await Task.Run(() => minLevenshteinComparator(fingerprints));
                        res = levResult.Item2;
                        similarity = levResult.Item1;
                    }
                }

                RegularExpression r = new RegularExpression(res.nama);

                List<string> names;
                IEnumerable<Person> allPersons;
                using (var connection = new MySqlConnection(connectionString))
                {
                    PersonRepository pr = new PersonRepository(connectionString);

                    allPersons = await pr.GetAllPersonsAsync();

                    names = allPersons.Select(person => person.nama).ToList();
                }
                Person tempPerson = allPersons.ToList()[0];

                String absolutePath = Path.GetFullPath(@"..\..\test\real\" + res.berkas_citra);
                Uri imageUri = new Uri(absolutePath);
                Console.WriteLine(imageUri);
                sidikJariHasil = new BitmapImage(imageUri);
                imageFound.Source = sidikJariHasil;
                stopwatch.Stop();

                textExTime.Text = $"Waktu eksekusi: {stopwatch.ElapsedMilliseconds} ms";
                similarity = similarity * 100;
                textPercentage.Text = $"Tingkat kemiripan: {similarity.ToString("F2")}%";
                biodata.Text = showPerson(tempPerson);

            }
        }

        private (bool, Fingerprint) parallelKmpComparator(List<Fingerprint> fingerprints)
        {
            var cts = new CancellationTokenSource();
            var options = new ParallelOptions { CancellationToken = cts.Token, MaxDegreeOfParallelism = Environment.ProcessorCount };
            bool found = false;
            Fingerprint foundFingerprint = null;
            object lockObj = new object();

            try
            {
                Parallel.ForEach(fingerprints, options, (fingerprint, state) =>
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        state.Stop();
                        return;
                    }

                    string absPath = Path.GetFullPath(Path.Combine(@"..\..\", "test", "real", fingerprint.berkas_citra));
                    if (File.Exists(absPath))
                    {
                        using (var bitmap = new Bitmap(absPath))
                        {
                            if (p.kmpComparator(absPath))
                            {
                                lock (lockObj)
                                {
                                    Console.WriteLine($"lagi nyari di {absPath}");
                                    if (!found)
                                    {
                                        found = true;
                                        foundFingerprint = fingerprint;
                                        cts.Cancel();
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (OperationCanceledException) { }

            return (found, foundFingerprint);
        }

        private (bool, Fingerprint) manualBmComparator(List<Fingerprint> fingerprints)
        {
            bool found = false;
            Fingerprint foundFingerprint = null;

            foreach (Fingerprint fingerprint in fingerprints)
            {
                string absPath = Path.GetFullPath(Path.Combine(@"..\..\", "test", "real", fingerprint.berkas_citra));
                if (File.Exists(absPath))
                {
                    {
                        Console.WriteLine($"lagi nyari di {absPath}");
                        if (p.bmComparator(absPath))
                        {
                            found = true;
                            foundFingerprint = fingerprint;
                            break;
                        }
                    }
                }
            }

            return (found, foundFingerprint);
        }

        private (bool, Fingerprint) parallelBmComparator(List<Fingerprint> fingerprints)
        {
            var cts = new CancellationTokenSource();
            var options = new ParallelOptions { CancellationToken = cts.Token, MaxDegreeOfParallelism = Environment.ProcessorCount };
            bool found = false;
            Fingerprint foundFingerprint = null;
            object lockObj = new object();

            try
            {
                Parallel.ForEach(fingerprints, options, (fingerprint, state) =>
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        state.Stop();
                        return;
                    }

                    string absPath = Path.GetFullPath(Path.Combine(@"..\..\", "test", "real", fingerprint.berkas_citra));
                    if (File.Exists(absPath))
                    {
                        {
                            Console.WriteLine($"lagi nyari di {absPath}");
                            Console.WriteLine(found);
                            if (p.bmComparator(absPath))
                            {
                                lock (lockObj)
                                {
                                    if (!found)
                                    {
                                        found = true;
                                        foundFingerprint = fingerprint;
                                        cts.Cancel();
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (OperationCanceledException) { }

            return (found, foundFingerprint);
        }


        private (double, Fingerprint) minLevenshteinComparator(List<Fingerprint> fingerprints)
        {
            double maxDistance = double.MinValue;
            Fingerprint minDistanceFingerprint = null;
            object lockObj = new object();

            foreach (Fingerprint fingerprint in fingerprints)
            {
                string absPath = Path.GetFullPath(Path.Combine(@"..\..\", "test", "real", fingerprint.berkas_citra));
                if (File.Exists(absPath))
                {
                    {
                        double distance = p.levComparator(absPath);

                        lock (lockObj)
                        {
                            if (distance > maxDistance)
                            {
                                maxDistance = distance;
                                minDistanceFingerprint = fingerprint;
                            }
                        }
                    }
                }
            }

            return (maxDistance, minDistanceFingerprint);
        }
    }
}