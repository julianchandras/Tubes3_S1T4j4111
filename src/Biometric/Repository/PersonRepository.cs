using Dapper;
using MySqlConnector;
using System.Data;
using Biometric.Models;
using System;

namespace Biometric.Repository
{
    class PersonRepository
    {
        private readonly string _connectionString;

        public PersonRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM biodata";
                return await db.QueryAsync<Person>(sql);
            }
        }

        public async Task<Person> GetPersonByNIKAsync(string nik)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM biodata WHERE NIK = @NIK";
                return await db.QueryFirstOrDefaultAsync<Person>(sql, new { NIK = nik });
            }
        }

        public async Task<Person> GetPersonByNameAsync(string _nama)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM biodata WHERE nama = @nama";
                return await db.QueryFirstOrDefaultAsync<Person>(sql, new { nama = _nama });
            }
        }

        public async Task<int> InsertPersonAsync(Person person)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) 
                           VALUES (@NIK, @Nama, @TempatLahir, @TanggalLahir, @JenisKelamin, @GolonganDarah, @Alamat, @Agama, @StatusPerkawinan, @Pekerjaan, @Kewarganegaraan)";
                return await db.ExecuteAsync(sql, person);
            }
        }

        public async Task<int> UpdatePersonAsync(Person person)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"UPDATE Person 
                           SET nama = @Nama, tempat_lahir = @TempatLahir, tanggal_lahir = @TanggalLahir, jenis_kelamin = @JenisKelamin, golongan_darah = @GolonganDarah, alamat = @Alamat, 
                               agama = @Agama, status_perkawinan = @StatusPerkawinan, pekerjaan = @Pekerjaan, kewarganegaraan = @Kewarganegaraan 
                           WHERE NIK = @NIK";
                return await db.ExecuteAsync(sql, person);
            }
        }

        public async Task<int> DeletePersonAsync(string nik)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM biodata WHERE NIK = @NIK";
                return await db.ExecuteAsync(sql, new { NIK = nik });
            }
        }
    }
}