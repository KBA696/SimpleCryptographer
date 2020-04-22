using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ПростойШифровальщик.ViewModel
{
    public class Crypto
    {
        ICryptoTransform CreateEncryptor;
        ICryptoTransform CreateDecryptor;
        public Crypto(string password)
        {
            using (var rijAlg = Rijndael.Create())
            {
                var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
                rijAlg.Key = pdb.GetBytes(32);
                rijAlg.IV = pdb.GetBytes(16);

                // Encrypt the string to an array of bytes.
                CreateEncryptor = rijAlg.CreateEncryptor();
                CreateDecryptor = rijAlg.CreateDecryptor();
            }

        }


        /// <summary>
        /// Зашифровка
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public byte[] EncryptStringToBytes(byte[] plainText)
        {
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, CreateEncryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// Разшифровка
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public byte[] DecryptStringFromBytes(byte[] cipherText)
        {
            // Declare the string used to hold
            // the decrypted text.
            byte[] plaintext = null;

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, CreateDecryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream msDecrypt1 = new MemoryStream())
                        {
                            int i = 0;
                            while ((i = csDecrypt.ReadByte()) != -1)
                            {
                                msDecrypt1.WriteByte((byte)i);
                            }
                            plaintext = msDecrypt1.ToArray();
                        }
                    }
                }

            return plaintext;
        }
    }
}