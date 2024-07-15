using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LicenseManagement.Helpers
{
    public class LicenseHelper
    {
        public RsaHelper Rsa = new RsaHelper();
        public RsaSecretKey SecretKey { get; set; }

        public LicenseHelper()
        {
            SecretKey = Rsa.GenerateRsaKeyPair();
        }

        public void GenerateKeyPair()
        {
            SecretKey = Rsa.GenerateRsaKeyPair();
        }
        public EncryptDetails Encrypt(License license, string aesKey = "")
        {
            if (aesKey.Length < 16) aesKey = Guid.NewGuid().ToString("N").Substring(16);
            var json = JsonConvert.SerializeObject(license);
            var aesContent = AesHelper.Encrypt(json, aesKey);
            Rsa.GetHash(aesContent, out string hashData);
            Rsa.SignatureFormatter(SecretKey.PrivateKey, hashData, out string signatureData);
            var result = new EncryptDetails
            {
                AesContent = aesContent,
                AesLength = aesContent.Length,
                AesKey = aesKey,
                RsaContent = signatureData
            };
            return result;
        }

        public License Decrypt(EncryptDetails details)
        {
            Rsa.GetHash(details.AesContent, out string hashData);
            var flag = Rsa.SignatureDeformatter(SecretKey.PublicKey, hashData, details.RsaContent);
            var license = JsonConvert.DeserializeObject<License>(AesHelper.Decrypt(details.AesContent, details.AesKey));
            return license;
        }
    }

    public class EncryptDetails
    {
        public string AesKey { get; set; }
        public int AesLength { get; set; }
        public string AesContent { get; set; }
        public string RsaContent { get; set; }
        public EncryptDetails(string str)
        {
            AesKey = str.Substring(0, 16);
            AesLength = Convert.ToInt32(str.Substring(16, 4), 16);
            AesContent = str.Substring(20, AesLength);
            RsaContent = str.Substring(20 + AesLength);
        }

        public EncryptDetails()
        {

        }

        public override string ToString()
            => AesKey + AesLength.ToString("X").PadLeft(4,'0') + AesContent + RsaContent;
    }

    public class License
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public string MachineCode { get; set; }
        public bool IsBlock { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Count { get; set; }
        public string DisableFunctionList { get; set; }
        public string DisableVersionList { get; set; }
    }
}
