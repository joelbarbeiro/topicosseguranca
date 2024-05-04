using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace chat
{
    public static class CryptFunctions
    {
        public static void keyGen()
        {
            // Create a new RSA instance
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Get the public and private key
                string publicKey = rsa.ToXmlString(false); // false for public key
                string privateKey = rsa.ToXmlString(true); // true for private key

                // Output the keys
                Console.WriteLine("Public Key:");
                Console.WriteLine(publicKey);
                Console.WriteLine();

                Console.WriteLine("Private Key:");
                Console.WriteLine(privateKey);
            }
        }
        public static byte[] encryptText(string text, RSACryptoServiceProvider rsa)
        {
            Console.WriteLine(text);
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(text);
            byte[] cypheredText = rsa.Encrypt(plaintextBytes, false);
            Console.WriteLine(cypheredText);

            return cypheredText;
        }
        public static string decryptText(byte[] text, RSACryptoServiceProvider rsa)
        {
            byte[] cryptedText = rsa.Decrypt(text, false);
            string plainText = Encoding.UTF8.GetString(cryptedText);
            Console.WriteLine(plainText);
            return plainText;
        }
    }
}
