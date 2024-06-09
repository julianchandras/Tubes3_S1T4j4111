using Biometric.Algorithms;
using Biometric.Models;
using Biometric.Repository;
using DotNetEnv;
using Microsoft.Win32;
using MySqlConnector;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Biometric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private BitmapImage sidikJari = null;
        private string algorithm = "BM";

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

        private async void start_Click (object sender, RoutedEventArgs e)
        {
            if (sidikJari == null)
            {
                MessageBox.Show("Mohon pilih sebuah gambar");
            } 
            else
            {
                if (radioButton2.IsChecked == true) { algorithm = "KMP"; }
                RegularExpression r = new RegularExpression("Hartana Wacana");

                Env.Load("..\\..\\.env");
                
                string server = Env.GetString("DB_SERVER");
                string user = Env.GetString("DB_USER");
                string password = Env.GetString("DB_PASS");
                string database = Env.GetString("DB_DATABASE");

                string connectionString = $"Server={server};User={user};Password={password};Database={database};";

                List<string> names;
                IEnumerable<Person> allPersons;
                using (var connection = new MySqlConnection(connectionString))
                {
                    PersonRepository pr = new PersonRepository(connectionString);

                    allPersons = await pr.GetAllPersonsAsync();

                    names = allPersons.Select(person => person.nama).ToList();
                }
                Person tempPerson = allPersons.ToList()[0];


                textExTime.Text = "Waktu eksekusi: 5 ms";
                textPercentage.Text = "Tingkat kemiripan: 100%";
                biodata.Text = showPerson(tempPerson);
            }
        }
    }
}