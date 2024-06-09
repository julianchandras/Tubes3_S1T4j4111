using Biometric.Models;
using Biometric.Repository;
using DotNetEnv;
using Microsoft.Win32;
using MySqlConnector;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                    Nama = "John Doe",
                    TempatLahir = "Jakarta",
                    TanggalLahir = new DateTime(1990, 1, 1), // Example date of birth
                    JenisKelamin = "Laki-Laki",
                    GolonganDarah = "O",
                    Alamat = "Jl. Sudirman No. 10",
                    Agama = "Islam",
                    StatusPerkawinan = "Belum Menikah",
                    Pekerjaan = "Developer",
                    Kewarganegaraan = "Indonesia",
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
            ret += "Nama: " + p.Nama + "\n";
            ret += "Tempat lahir: " + p.TempatLahir + "\n";
            DateTime dt = p.TanggalLahir.GetValueOrDefault();
            ret += "Tanggal lahir: " + dt.Day + "-" + dt.Month + "-" + dt.Year + "\n";
            ret += "Jenis Kelamin: " + p.JenisKelamin + "\n";
            ret += "Alamat: " + p.Alamat + "\n";
            ret += "Agama: " + p.Agama + "\n";
            ret += "Status Perkawinan: " + p.StatusPerkawinan + "\n";
            ret += "Pekerjaan: " + p.Pekerjaan + "\n";
            ret += "Kewarganegaraan: " + p.Kewarganegaraan;

            return ret;
        }

        private void start_Click (object sender, RoutedEventArgs e)
        {
            if (sidikJari == null)
            {
                MessageBox.Show("Mohon pilih sebuah gambar");
            } 
            else
            {
                if (radioButton2.IsChecked == true) { algorithm = "KMP"; }
                Person tempPerson = new Person
                {
                    NIK = "13522080",  // Assuming this is the NIK
                    Nama = "Julian Chan",
                    TempatLahir = "Tangerang",
                    TanggalLahir = new DateTime(2004, 4, 10), // Example date of birth
                    JenisKelamin = "Laki-Laki",
                    GolonganDarah = "O",
                    Alamat = "Jl. Sudirman No. 10",
                    Agama = "Katolik",
                    StatusPerkawinan = "HTS",
                    Pekerjaan = "Mahasiswa",
                    Kewarganegaraan = "Indonesia",
                };

                textExTime.Text = "Waktu eksekusi: 5 ms";
                textPercentage.Text = "Tingkat kemiripan: 100%";
                biodata.Text = showPerson(tempPerson);
            }
        }
    }
}