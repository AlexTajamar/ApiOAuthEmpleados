namespace ApiOAuthEmpleados.Helpers
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class HelperCifrado
    {
        public static async Task<string> EncryptStringAsync(string input, string clave)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(clave.PadRight(32).Substring(0, 32));
            byte[] iv = new byte[16];

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;

            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            
            await cs.WriteAsync(inputBytes, 0, inputBytes.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        public static async Task<string> DecryptStringAsync(string input, string clave)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(clave.PadRight(32).Substring(0, 32));
            byte[] iv = new byte[16];

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;

            byte[] inputBytes = Convert.FromBase64String(input);

            using MemoryStream ms = new MemoryStream(inputBytes);
            using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);
            
            return await sr.ReadToEndAsync();
        }
    }
}
