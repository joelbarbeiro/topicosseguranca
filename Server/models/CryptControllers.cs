using System;
using System.Collections.Generic;
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
    }
}
