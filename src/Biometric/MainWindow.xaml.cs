using System.IO;
using System.Windows;
using Dapper;
using DotNetEnv;
using MySqlConnector;
using Biometric.Models;
using Biometric.Repository;

namespace Biometric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Mark the constructor as async and change its return type to Task
            _ = InitializeAsync();
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
                PersonRepository pr = new PersonRepository(connectionString);

                Person test = new Person("13522080")
                {
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

                int rowsAffected = await pr.InsertPersonAsync(test);

                MessageBox.Show(rowsAffected.ToString());
                IEnumerable<Person> allPersons = await pr.GetAllPersonsAsync();
                    
                // Convert rowsAffected to string before showing it in MessageBox
                MessageBox.Show(((allPersons.First()).NIK).ToString());
            }
        }
    }
}