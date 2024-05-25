using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometric.Models
{
    class Person
    {
        public string? NIK { get; set; }
        public string? Nama { get; set; }
        public string? TempatLahir { get; set; }
        public DateTime? TanggalLahir { get; set; }  // Nullable DateTime for nullable 'tanggal_lahir' field
        public string? JenisKelamin { get; set; }
        public string? GolonganDarah { get; set; }
        public string? Alamat { get; set; }
        public string? Agama { get; set; }
        public string? StatusPerkawinan { get; set; }
        public string? Pekerjaan { get; set; }
        public string? Kewarganegaraan { get; set; }

/*        public Person(string nik)
        {
            NIK = nik ?? throw new ArgumentNullException(nameof(nik));
        }*/
    }
}