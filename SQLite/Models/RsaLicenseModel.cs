using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagement.SQLite.Models
{
    public class RsaLicenseModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string MachineCode { get; set; }
        public bool IsBlock { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Count { get; set; }
        public string DisableFunctionList { get; set; }
        public string DisableVersionList { get; set; }
        public string Code { get; set; }
        public DateTime CreateDate { get; set; }
        public string OpName { get; set; }
    }
}
