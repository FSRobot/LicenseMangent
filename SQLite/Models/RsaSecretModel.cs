using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagement.SQLite.Models
{
    public class RsaSecretModel
    {
        public Guid Id { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
