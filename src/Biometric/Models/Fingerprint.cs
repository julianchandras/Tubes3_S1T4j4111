using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometric.Models
{
    class Fingerprint
    {
        public string citra { get; set; }
        public string nama { get; set; }

        public Fingerprint(string nama, string citra)
        {
            this.nama = nama;
            this.citra = citra;
        }
    }
}
