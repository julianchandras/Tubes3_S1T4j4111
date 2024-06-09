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
        public string? nama { get; set; }
        public string? tempat_lahir { get; set; }
        public DateTime? tanggal_lahir { get; set; }  // Nullable DateTime for nullable 'tanggal_lahir' field
        public string? jenis_kelamin { get; set; }
        public string? golongan_darah { get; set; }
        public string? alamat { get; set; }
        public string? agama { get; set; }
        public string? status_perkawinan { get; set; }
        public string? pekerjaan { get; set; }
        public string? kewarganegaraan { get; set; }

/*        public Person(string nik)
        {
            NIK = nik ?? throw new ArgumentNullException(nameof(nik));
        }*/
    }
}