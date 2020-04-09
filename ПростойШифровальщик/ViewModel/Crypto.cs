using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ПростойШифровальщик.ViewModel
{
    class Crypto
    {
        public static byte[] EncryptStringToBytes(byte[] plainText, SymmetricAlgorithm symmetricAlgorithm)
        {
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
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

        public static byte[] DecryptStringFromBytes(byte[] cipherText, SymmetricAlgorithm symmetricAlgorithm)
        {
            // Declare the string used to hold
            // the decrypted text.
            byte[] plaintext = null;

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read))
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