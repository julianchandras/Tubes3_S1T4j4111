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
        private BitmapImage sidikJari = new BitmapImage();
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
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Load the selected image file into the image control
                    BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    imageControl.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        private void start_Click (object sender, RoutedEventArgs e)
        {
            // Initialize the application
            if (radioButton2.IsChecked == true) { algorithm = "KMP"; }
            MessageBox.Show(algorithm);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}