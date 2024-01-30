using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Privont
{
    public class Encryption
    {
        public static class StringCipher
        {
            //private const string EncryptionKey = "YourSecretEncryptionKey"; // Change this to a strong, secret key

            public static string Encrypt(string plainText, string key)
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                for (int i = 0; i < plainBytes.Length; i++)
                {
                    plainBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
                }

                return Convert.ToBase64String(plainBytes);
            }

            public static string Decrypt(string encryptedText, string key)
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                for (int i = 0; i < encryptedBytes.Length; i++)
                {
                    encryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
                }

                return Encoding.UTF8.GetString(encryptedBytes);
            }
        }

    }
}