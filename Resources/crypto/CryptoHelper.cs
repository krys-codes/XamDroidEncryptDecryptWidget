using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptDecryptWidget.Resources.crypto
{
    public class CryptoHelper
    {
        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="decryptTxt">Text you would like to decrypt</param>
        /// <param name="key">Your decrypt key</param>
        /// <returns></returns>
        public string PasswordDecrypt(string decryptTxt, string key)
        {
            decryptTxt = decryptTxt.Replace(" ", "+");
            var bytesBuff = Convert.FromBase64String(decryptTxt);
            using (var aes = Aes.Create())
            {
                var crypto = new Rfc2898DeriveBytes(key,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (var mStream = new MemoryStream())
                {
                    using (var cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }

                    decryptTxt = Encoding.Unicode.GetString(mStream.ToArray());
                }
            }
            return decryptTxt;
        }

        /// <summary>
        /// Encrypt string
        /// </summary>
        /// <param name="encryptTxt">Text you would like to encrypt</param>
        /// <param name="key">Your encrypt key</param>
        /// <returns></returns>
        public string PasswordEncrypt(string encryptTxt, string key)
        {
            var bytesBuff = System.Text.Encoding.Unicode.GetBytes(encryptTxt);
            using (var aes = Aes.Create())
            {
                var crypto = new Rfc2898DeriveBytes(key,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (var mStream = new MemoryStream())
                {
                    using (var cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }

                    encryptTxt = Convert.ToBase64String(mStream.ToArray());
                }
            }

            return encryptTxt;
        }
    }
}