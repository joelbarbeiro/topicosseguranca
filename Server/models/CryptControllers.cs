using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.models
{
    public class CryptControllers
    { 
        public static void keyGen(out string pubKey, out string privKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters privateKeyParams = rsa.ExportParameters(true);
                privKey = rsa.ToXmlString(true);

                RSAParameters publicKeyParams = rsa.ExportParameters(false);
                pubKey = rsa.ToXmlString(false);

                rsa.PersistKeyInCsp = false;
            }

        }
        public static string encryptText(string text, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(key);

                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);
                    byte[] cypheredText = rsa.Encrypt(plainTextBytes, true);

                    rsa.PersistKeyInCsp = false;

                    return Convert.ToBase64String(cypheredText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERRO AO ENCRIPTAR NO CLIENTE -->> " + ex);
                    return string.Empty;
                }
            }
        }

        public static string decryptText(string text, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(key);
                    byte[] cryptedText = Convert.FromBase64String(text);
                    byte[] plainTextByte = rsa.Decrypt(cryptedText, true);

                    rsa.PersistKeyInCsp = false;

                    return Encoding.UTF8.GetString(plainTextByte);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DECRYPT " + ex);
                    return null;
                }
            }
        }
        public static string GenHash(string text)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] bytesText = Encoding.UTF8.GetBytes(text);

                byte[] hash = sha1.ComputeHash(bytesText);

                return Convert.ToBase64String(hash);
            }
        }

        public static string SignedHash(string hash, string key)
        {
            using (RSACryptoServiceProvider rsaSign = new RSACryptoServiceProvider())
            {
                rsaSign.FromXmlString(key);

                byte[] byteHash = Convert.FromBase64String(hash);

                byte[] signature = rsaSign.SignHash(byteHash, CryptoConfig.MapNameToOID("SHA1"));

                return Convert.ToBase64String(signature);
            }
        }

        public static bool VerifyHash(string signHash, string hash, string key)
        {
            using (RSACryptoServiceProvider rsaVerify = new RSACryptoServiceProvider())
            {
                rsaVerify.FromXmlString(key);

                byte[] bytesSigHash = Convert.FromBase64String(signHash);

                byte[] byteHash = Convert.FromBase64String(hash);

                bool val1 = rsaVerify.VerifyHash(byteHash, CryptoConfig.MapNameToOID("SHA1"), bytesSigHash);


                return val1;
            }
        }

        // AES Encryption
        public static string AESEncrypt(string plainText)
        {
            string encryptedText = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes("770A8A65DA156D24EE2A093277530142");
                aesAlg.IV = Encoding.UTF8.GetBytes("F5502320F8429037");
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    encryptedText = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }

            return encryptedText;
        }

        // AES Decryption
        public static string AESDecrypt(string cipherText)
        {
            string plaintext = null;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes("770A8A65DA156D24EE2A093277530142");
                aesAlg.IV = Encoding.UTF8.GetBytes("F5502320F8429037");

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

    }
}
