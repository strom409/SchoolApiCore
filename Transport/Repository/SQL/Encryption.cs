using System.Security.Cryptography;

namespace Transport.Repository.SQL
{
    public class Encryption
    {

        private static byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };
        public static string key = "$xdsv#bskb@ksjdckshd&kjsb)%(dkb$ksjd++++bdjhsd^bksdv";
        /// <summary>
        /// Encrypt a string using a password
        /// </summary>
        public static string Encrypt(string plainText, string password)
        {
            // Turn the input string into a byte array 
            byte[] plainBytes = System.Text.Encoding.Unicode.GetBytes(plainText);

            // Derive a key from the passowrd and salt
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, salt);

            // Encrypt
            byte[] encryptedData = Encrypt(plainBytes, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypt a string using a password 
        /// </summary>
        public static string Decrypt(string encryptedText, string password)
        {
            // Turn the input string into a byte array 
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            // Derive a key from the passowrd and salt
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt);

            // Decrypt
            byte[] decryptedBytes = Decrypt(encryptedBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            // Turn the byte array into a string
            return System.Text.Encoding.Unicode.GetString(decryptedBytes);
        }

        /// <summary>
        /// Encrypt a byte array
        /// </summary>
        public static byte[] Encrypt(byte[] clearData, byte[] key, byte[] initialisationVector)
        {
            // Create a MemoryStream
            MemoryStream memoryStream = new MemoryStream();

            // Create a symmetric algorithm 
            Rijndael algorithm = Rijndael.Create();
            algorithm.Key = key;
            algorithm.IV = initialisationVector;

            // Create a CryptoStream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the encryption 
            cryptoStream.Write(clearData, 0, clearData.Length);
            cryptoStream.Close();

            // Convert the encrypted stream to a byte array 
            byte[] encryptedData = memoryStream.ToArray();

            return encryptedData;
        }

        /// <summary>
        /// Decrypt a byte array 
        /// </summary>
        public static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] initialisationVector)
        {
            // Create a MemoryStream
            MemoryStream memoryStream = new MemoryStream();

            // Create a symmetric algorithm 
            Rijndael algorithm = Rijndael.Create();

            // Now set the key and the IV. 
            algorithm.Key = key;
            algorithm.IV = initialisationVector;

            // Create a CryptoStream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, algorithm.CreateDecryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the decryption 
            cryptoStream.Write(cipherData, 0, cipherData.Length);
            cryptoStream.Close();

            // Convert the decrypted stream to a byte array 
            byte[] decryptedData = memoryStream.ToArray();

            return decryptedData;
        }
    }
}
