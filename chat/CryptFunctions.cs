using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    public static class CryptFunctions
    {
        public static string genPassHash(string pass)
        {
            byte[] salt = new byte[] { 0, 1, 0, 8, 9, 2, 3, 5 };
            Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes(pass, salt, 1000);

            byte[] key = pwdGen.GetBytes(16);

            string pass64 = Convert.ToBase64String(key);

            return pass64;
        }
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
                    Console.WriteLine("Plain text to encrypt --> " + text);
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);
                    byte[] cypheredText = rsa.Encrypt(plainTextBytes, true);
                    rsa.PersistKeyInCsp = false;

                    return Convert.ToBase64String(cypheredText);
                }
                catch(Exception ex)
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
                    string plainText = Encoding.UTF8.GetString(plainTextByte);

                    rsa.PersistKeyInCsp = false;

                    return plainText;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("DECRYPT " + ex);
                    return null;
                }
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

