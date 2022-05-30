using System;
using System.Security.Cryptography;

namespace SecurePasswordManager
{
    class Program
    {
        static void Main(string[] args)
        {
            RunSecurePasswordManager();
        }

        private static void RunSecurePasswordManager()
        {
            string hash = Convert.ToBase64String(CreateHash("Test123"));
            Console.WriteLine(hash);
            
            throw new NotImplementedException();

        }

        private static byte[] CreateHash(string input)
        {
            // Generate a salt
            const int SALT_SIZE = 24; // size in bytes
            const int HASH_SIZE = 24; // size in bytes
            const int ITERATIONS = 100000; // number of pbkdf2 iterations
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_SIZE];
            provider.GetBytes(salt);

            // Generate the hash
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, ITERATIONS);
            return pbkdf2.GetBytes(HASH_SIZE);
        }
    }
}
