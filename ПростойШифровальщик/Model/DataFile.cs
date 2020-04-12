using System;

namespace ПростойШифровальщик.Model
{
    [Serializable]
    public class DataFile
    {
        /// <summary>
        /// Зашифрованный класс
        /// </summary>
        public byte[] EncryptedClass { get; set; }
        /// <summary>
        /// Зашифрованный класс
        /// </summary>
        public byte[] EncryptedFiles { get; set; }

    }
}
