using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagement.SQLite.Models
{
    public class OemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SecretKey { get; set; }
        public string PublickKey { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDeleted { get; set; }
        public Guid OpId { get; set; }
    }
}
