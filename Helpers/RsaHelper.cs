using System;
using System.Security.Cryptography;
using System.Text;

namespace LicenseManagement.Helpers
{
    /// <summary>
    /// Rsa秘钥
    /// </summary>
    public struct RsaSecretKey
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public override string ToString()
        {
            return $"PrivateKey: {PrivateKey}\r\nPublicKey: {PublicKey}";
        }
    }

    public class RsaHelper
    {
        static string Algorithm = "MD5";
        #region RSA 加密解密

        #region RSA 的密钥产生

        /// <summary>
        /// 生成Rsa秘钥对
        /// </summary>
        /// <returns></returns>
        public RsaSecretKey GenerateRsaKeyPair()
        {
            var rsa = new RSACryptoServiceProvider(8192);
            var result = new RsaSecretKey
            {
                PrivateKey = rsa.ToXmlString(true),
                PublicKey = rsa.ToXmlString(false)
            };
            return result;
        }

        #endregion

        #region RSA加密函数

        /// <summary>  
        /// RSA的加密函数  
        /// </summary>  
        /// <param name="xmlPublicKey">公钥</param>  
        /// <param name="encryptString">待加密的字符串</param>  
        /// <returns></returns>  
        public string RsaEncrypt(string xmlPublicKey, string encryptString)
        {
            var rsa = new RSACryptoServiceProvider(8192);
            rsa.FromXmlString(xmlPublicKey);
            var plainTextBArray = (new UnicodeEncoding()).GetBytes(encryptString);
            var cypherTextBArray = rsa.Encrypt(plainTextBArray, false);
            return Convert.ToBase64String(cypherTextBArray);
        }

        /// <summary>  
        /// RSA的加密函数   
        /// </summary>  
        /// <param name="xmlPublicKey">公钥</param>  
        /// <param name="encryptStr">待加密的字节数组</param>  
        /// <returns></returns>  
        public string RsaEncrypt(string xmlPublicKey, byte[] encryptStr)
        {
            var rsa = new RSACryptoServiceProvider(8192);
            rsa.FromXmlString(xmlPublicKey);
            var cypherTextBArray = rsa.Encrypt(encryptStr, false);
            return Convert.ToBase64String(cypherTextBArray);
        }

        #endregion

        #region RSA的解密函数

        /// <summary>  
        /// RSA的解密函数  
        /// </summary>  
        /// <param name="xmlPrivateKey">私钥</param>  
        /// <param name="decryptString">待解密的字符串</param>  
        /// <returns></returns>  
        public string RsaDecrypt(string xmlPrivateKey, string decryptString)
        {
            var rsa = new RSACryptoServiceProvider(8192);
            rsa.FromXmlString(xmlPrivateKey);
            var plainTextBArray = Convert.FromBase64String(decryptString);
            var decryptTextBArray = rsa.Decrypt(plainTextBArray, false);
            return (new UnicodeEncoding()).GetString(decryptTextBArray);
        }

        /// <summary>  
        /// RSA的解密函数   
        /// </summary>  
        /// <param name="xmlPrivateKey">私钥</param>  
        /// <param name="decryptString">待解密的字节数组</param>  
        /// <returns></returns>  
        public string RsaDecrypt(string xmlPrivateKey, byte[] decryptString)
        {
            var rsa = new RSACryptoServiceProvider(8192);
            rsa.FromXmlString(xmlPrivateKey);
            var decryptTextBArray = rsa.Decrypt(decryptString, false);
            return (new UnicodeEncoding()).GetString(decryptTextBArray);
        }

        #endregion

        #endregion

        #region RSA数字签名

        #region 获取Hash描述表

        /// <summary>  
        /// 获取Hash描述表  
        /// </summary>  
        /// <param name="strSource">待签名的字符串</param>  
        /// <param name="hashData">Hash描述</param>  
        /// <returns></returns>  
        public bool GetHash<T>(string strSource, out T hashData)
        {
            var md5 = HashAlgorithm.Create(Algorithm);
            if (md5 == null)
                throw new ArgumentException("can't find algorithm!", nameof(Algorithm));

            var buffer = Encoding.GetEncoding("UTF-8").GetBytes(strSource);
            if (typeof(T) == typeof(byte[]))
                hashData = (T)(object)md5.ComputeHash(buffer);
            else if (typeof(T) == typeof(string))
                hashData = (T)(object)Convert.ToBase64String(md5.ComputeHash(buffer));
            else
                hashData = default;

            return true;
        }

        /// <summary>  
        /// 获取Hash描述表  
        /// </summary>  
        /// <param name="objFile">待签名的文件</param>  
        /// <param name="hashData">Hash描述</param>  
        /// <returns></returns>  
        public bool GetHash<T>(System.IO.FileStream objFile, out T hashData)
        {
            var md5 = HashAlgorithm.Create(Algorithm);
            if (md5 == null)
                throw new ArgumentException("can't find algorithm!", nameof(Algorithm));

            var computeHash = md5.ComputeHash(objFile);
            if (typeof(T) == typeof(byte[]))
                hashData = (T)(object)computeHash;
            else if (typeof(T) == typeof(string))
                hashData = (T)(object)Convert.ToBase64String(computeHash);
            else
                hashData = default;

            objFile.Close();
            return true;
        }
        #endregion


        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strKeyPrivate">私钥</param>
        /// <param name="hashSignature">待签名Hash描述</param>
        /// <param name="encryptedSignatureData">签名后的结果</param>
        /// <returns></returns>  
        public bool SignatureFormatter<T1, T2>(string strKeyPrivate, T1 hashSignature, out T2 encryptedSignatureData)
        {
            byte[] bytes;
            if (typeof(T1) == typeof(string))
                bytes = Convert.FromBase64String((string)(object)hashSignature);
            else
                bytes = (byte[])(object)hashSignature;

            var rsa = new RSACryptoServiceProvider(8192);
            rsa.FromXmlString(strKeyPrivate);
            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm("MD5");

            var signature = rsaFormatter.CreateSignature(bytes);
            if (typeof(T2) == typeof(byte[]))
                encryptedSignatureData = (T2)(object)signature;
            else if (typeof(T2) == typeof(string))
                encryptedSignatureData = (T2)(object)Convert.ToBase64String(signature);
            else
                encryptedSignatureData = default;

            return true;
        }
        /// <summary>  
        /// RSA签名验证  
        /// </summary>  
        /// <param name="strKeyPublic">公钥</param>  
        /// <param name="hash">Hash描述</param>  
        /// <param name="deformatterData">签名后的结果</param>  
        /// <returns></returns>  
        public bool SignatureDeformatter<T1, T2>(string strKeyPublic, T1 hash, T2 deformatterData)
        {
            byte[] bytes;
            if (typeof(T1) == typeof(string))
                bytes = Convert.FromBase64String((string)(object)hash);
            else
                bytes = (byte[])(object)hash;

            var rsa = new RSACryptoServiceProvider(8192);
            rsa.FromXmlString(strKeyPublic);
            var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("MD5");
            if (typeof(T2) == typeof(byte[]))
                return rsaDeformatter.VerifySignature(bytes, (byte[])(object)deformatterData);

            return rsaDeformatter.VerifySignature(bytes, Convert.FromBase64String((string)(object)deformatterData));
        }

        #endregion
    }
}