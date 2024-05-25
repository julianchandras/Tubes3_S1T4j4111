using Dapper;
using Biometric.Models;
using MySqlConnector;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Biometric.Repository
{
    class FingerprintRepository
    {
        private readonly string _connnectionString;

        public FingerprintRepository(string connectionString)
        {
            _connnectionString = connectionString;
        }

        public async Task<IEnumerable<Fingerprint>> GetAllFingerprintAsync()
        {
            using (IDbConnection db = new MySqlConnection(_connnectionString))
            {
                string sql = "SELECT * FROM fingerprint";
                return await db.QueryAsync<Fingerprint>(sql);
            }
        }

        public async Task<int> InsertFingerprintAsync(Fingerprint fingerprint)
        {
            using (IDbConnection db = new MySqlConnection(_connnectionString))
            {
                string sql = @"INSERT INTO sidik_jari (berkas_citra, nama)
                            VALUES (@sidik_jari, @nama)";
                return await db.ExecuteAsync(sql, fingerprint);

            }
        }
    }
}