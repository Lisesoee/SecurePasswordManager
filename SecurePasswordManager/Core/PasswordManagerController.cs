using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SecurePasswordManager.Core
{
    internal class PasswordManagerController
    {
        
        internal static void CreateNewItem(string name, string username, string password)
        {
            throw new NotImplementedException();

            string passwordHash = Convert.ToBase64String(PasswordManagerController.CreateHash(password));
        }

        public static byte[] CreateHash(string input)
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

        internal static void GetItem(string name)
        {
            throw new NotImplementedException();
        }

        internal static void CreateNewUser(string userNameInput, string masterPasswordInput)
        {
            throw new NotImplementedException();
        }
    }
}
