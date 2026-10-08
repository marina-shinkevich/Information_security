using System;
using System.Security.Cryptography;

public class CustomDES
{
    public byte[] Encrypt(byte[] plainText, byte[] key)
    {
        using (var des = new DESCryptoServiceProvider())
        {
            des.Key = key; // Прямое использование ключа, даже если он слабый
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            using (var encryptor = des.CreateEncryptor())
            {
                return encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
            }
        }
    }

    public byte[] Decrypt(byte[] cipherText, byte[] key)
    {
        using (var des = new DESCryptoServiceProvider())
        {
            des.Key = key; // Прямое использование ключа, даже если он слабый
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            using (var decryptor = des.CreateDecryptor())
            {
                return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            }
        }
    }
}