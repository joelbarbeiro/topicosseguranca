using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    public static class CryptFunctions
    {
        public static void keyGen(out string pubKey, out string privKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Export the public key
                RSAParameters publicKeyParams = rsa.ExportParameters(false); // false indicates exporting the public key
                pubKey = rsa.ToXmlString(false); // Export to XML string

                // Export the private key
                RSAParameters privateKeyParams = rsa.ExportParameters(true); // true indicates exporting the private key
                privKey = rsa.ToXmlString(true); // Export to XML string
            }
        }

        public static string encryptText(string text, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                Console.WriteLine(text);
                byte[] plaintextBytes = Encoding.UTF8.GetBytes(text);
                byte[] cypheredText = rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(cypheredText);
            }

        }

        public static string decryptText(string text, string key)
        {
            string plainText = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()) {
                try
                {
                    rsa.FromXmlString(key);
                    byte[] cryptedText = Convert.FromBase64String(text);
                    byte[] plainTextByte = rsa.Decrypt(cryptedText, RSAEncryptionPadding.Pkcs1);
                    plainText = Encoding.UTF8.GetString(plainTextByte);
                    return plainText;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("DECRYPT " + ex);
                    return null;
                }
            }
        }
    }

}

