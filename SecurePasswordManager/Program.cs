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
            bool userExists = false;

            if (userExists == true)
            {
                if (Login() == true) {
                    MainMenu();
                }
            }
            else
            {
                CreateMasterPassword();
            }
 
            
            throw new NotImplementedException();

        }

        private static void CreateMasterPassword()
        {
            Console.WriteLine("Please input a master password.");
            string masterPasswordInput = Console.ReadLine();

            throw new NotImplementedException();
        }

        private static bool Login()
        {
            throw new NotImplementedException();

            bool forgotPassword = false;
            if (forgotPassword == true)
            {
                //ResetMasterPassword();
            }

            return false;
        }

        private static void MainMenu()
        {
            throw new NotImplementedException();

            // todo: create meny where user input specifies an action

            AddNewPassword();
            RetrievePassword();
        }
        

        private static void RetrievePassword()
        {
            throw new NotImplementedException();
        }

        private static void AddNewPassword()
        {
            throw new NotImplementedException();

            string hash = Convert.ToBase64String(CreateHash("Test123"));
            Console.WriteLine(hash);
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
