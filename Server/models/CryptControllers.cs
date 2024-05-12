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
        public static string encryptText(string text, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                Console.WriteLine("Palin text to encrypt --> " + text);
                byte[] plaintextBytes = Encoding.UTF8.GetBytes(text);
                byte[] cypheredText = rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.Pkcs1);
                Console.WriteLine(cypheredText);
                return Convert.ToBase64String(cypheredText);
            }
        }

        public static string decryptText(string text, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                byte[] cryptedText = Convert.FromBase64String(text);
                cryptedText = rsa.Decrypt(cryptedText, RSAEncryptionPadding.Pkcs1);
                string plainText = Encoding.UTF8.GetString(cryptedText);
                Console.WriteLine(plainText + "DECRYPT");
                return plainText;
            }
        }
        
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
