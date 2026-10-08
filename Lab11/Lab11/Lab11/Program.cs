using System;
using System.Security.Cryptography;
using System.Text;

namespace SHA256Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nХеширование SHA256");
            var startTime = DateTime.Now;

            // string text = GenerateRandomText(100000);
            string text = "Shinkevich Marina Dmitrievna";
            string salt = CreateSalt(15);
            string hash = GenerateSHA256(text, salt);

            Console.WriteLine($"Message: {text}\nХэш: {hash}");
            Console.WriteLine($"Время: {(DateTime.Now - startTime).TotalMilliseconds} мс\n\n");
        }

        public static string CreateSalt(int size)
        {
            using var rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string GenerateSHA256(string input, string salt)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            byte[] hash = sha256.ComputeHash(bytes);
            return ToHex(hash);
        }

        public static string ToHex(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public static string GenerateRandomText(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
            using var rng = new RNGCryptoServiceProvider();
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                byte[] randomBytes = new byte[4];
                rng.GetBytes(randomBytes);
                uint randomIndex = BitConverter.ToUInt32(randomBytes, 0) % (uint)characters.Length;
                sb.Append(characters[(int)randomIndex]);
            }

            return sb.ToString();
        }
    }
}