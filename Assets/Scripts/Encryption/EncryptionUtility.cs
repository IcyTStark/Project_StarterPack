using System;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionUtility
{
    private static readonly byte[] key = Encoding.UTF8.GetBytes("wlewb10UI2lMv3gZnf9CDf99YsPYfZ2v");

    public static string Encrypt(string inText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Generate a secure random IV
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(inText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                // Prepend the IV to the encrypted data
                byte[] result = new byte[iv.Length + encryptedBytes.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

                return Convert.ToBase64String(result);
            }
        }
    }

    public static string Decrypt(string inEncryptedText)
    {
        byte[] cipherBytes = Convert.FromBase64String(inEncryptedText);

        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Extract the IV from the cipher bytes
            byte[] iv = new byte[16]; // AES block size is 16 bytes
            byte[] encryptedBytes = new byte[cipherBytes.Length - iv.Length];

            Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(cipherBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
            {
                byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }
}