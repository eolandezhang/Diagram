using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace QPP.Security
{
    /// <summary>
    /// String encrypt/decrypt.
    /// </summary>
    public class Cryptography
    {
        /// <summary>
        /// MD5 encrypt.
        /// </summary>
        /// <param name="source">String to encrypt</param>
        /// <returns>String encrypted</returns>
        public static string Md5(string source)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(source);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }

        /// <summary>
        /// SHA1 encrypt.
        /// </summary>
        /// <param name="source">String to encrypt</param>
        /// <returns>String encrypted</returns>
        public static string Sha1(string source)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(source);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("SHA1")).ComputeHash(clearBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", ""); ;
        }

        /// <summary>
        /// Rijndael(AES) Encrypt.
        /// </summary>
        /// <param name="source">Message to encrypt</param>
        /// <param name="aesKey">Key</param>
        /// <returns>Message encrypted</returns>
        public static string AesEncrypt(string source, string aesKey, string aesIV)
        {
            byte[] bytIn = Encoding.UTF8.GetBytes(source);

            //Bulid the Rijndael Key.
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(aesKey.PadRight(bKey.Length)), bKey, bKey.Length);

            //Build the vector.
            byte[] bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(aesIV.PadRight(bVector.Length)), bVector, bVector.Length);

            //Create the Rijndael Object.
            Rijndael aes = Rijndael.Create();

            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream,
                    aes.CreateEncryptor(bKey, bVector),
                    CryptoStreamMode.Write))
                {
                    cStream.Write(bytIn, 0, bytIn.Length);
                    cStream.FlushFinalBlock();
                    return Convert.ToBase64String(mStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Rijndael(AES) Decrypt.
        /// </summary>
        /// <param name="source">Message to decrypt</param>
        /// <param name="aesKey">Key</param>
        /// <returns>Message decrypted</returns>
        public static string AesDecrypt(string source, string aesKey, string aesIV)
        {
            byte[] bytIn = Convert.FromBase64String(source);

            //Bulid the Rijndael Key.
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(aesKey.PadRight(bKey.Length)), bKey, bKey.Length);

            //Build the vector.
            byte[] bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(aesIV.PadRight(bVector.Length)), bVector, bVector.Length);

            //Create the Rijndael Object.
            Rijndael aes = Rijndael.Create();

            using (MemoryStream mStraem = new MemoryStream(bytIn))
            {
                using (CryptoStream cStream = new CryptoStream(mStraem,
                    aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                {
                    using (StreamReader sReader = new StreamReader(cStream))
                    {
                        return sReader.ReadToEnd();
                    }
                }
            }
        }
    }
}


