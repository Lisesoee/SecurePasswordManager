using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;
using SecurePasswordManager.Classes;
using System.Reflection;
using System.Security;
using System.IO;

namespace SecurePasswordManager.Core
{
    internal class PasswordManagerController
    {
        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\chaudrj\source\repos\Lisesoee\SecurePasswordManager\SecurePasswordManager\Database\PasswordVaultDB.mdf;Integrated Security=True;Connect Timeout=30";
        private const int SALT_SIZE = 24; // size in bytes
        private const int HASH_SIZE = 24; // size in bytes
        private const int ITERATIONS = 100000; // number of pbkdf2 iterations
        private static byte[] salt = new byte[SALT_SIZE] { 0x33, 0xBA, 0x00, 0x3C, 0x1F, 0x35, 0xEC, 0xC3, 0xBE, 0xA3, 0x78, 0x7C, 0xEF, 0x9B, 0xA2, 0xC6, 0x9B, 0xC3, 0x13, 0x39, 0x99, 0x95, 0xF5, 0x63 };
        private static byte[] aes_key = new byte[] { 0xCB, 0xA6, 0x56, 0x20, 0xD8, 0x22, 0xCA, 0x26, 0xE9, 0x7B, 0xFE, 0x63, 0x62, 0xE1, 0x7C, 0xF2, 0x32, 0x38, 0x38, 0xD0, 0x06, 0x5F, 0x28, 0x8E, 0xA1, 0x87, 0x2C, 0x03, 0xC1, 0x90, 0x96, 0x0D };
        private static byte[] IV_aes = new byte[16];
        public static byte[] CreateHash(string input)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            // Generate the hash
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, ITERATIONS);
            
            return pbkdf2.GetBytes(HASH_SIZE);
        }

        

        public static bool CreateNewUser(string userNameInput, string masterPasswordInput)
        {
            // todo: sanitize inputs

            string masterPasswordHash = Convert.ToBase64String(PasswordManagerController.CreateHash(masterPasswordInput));
            using (SqlConnection cs = new SqlConnection(connectionString))
            {
                
                SqlCommand cmd = new SqlCommand("INSERT INTO MasterPassword (MasterPassword, UserName) values (@masterPasswordInput,@usernameinput)", cs);
                cmd.Parameters.AddWithValue("@usernameinput", userNameInput);
                cmd.Parameters.AddWithValue("@masterPasswordInput", masterPasswordHash);
                cs.Open();
                cmd.ExecuteNonQuery();
                cs.Close();
                return true;
                
            }
        }
        public static bool CreateNewItem(string name, string username, string password)
        {
            // todo: sanitize inputs

            // todo: encrypt data before storing it
            using (SqlConnection cs = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO VaultItem (Name, UserName, Password) values (@name,@username,@password)", cs);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", Convert.ToBase64String(Encrypt(password, aes_key, IV_aes)));
                cs.Open();
                cmd.ExecuteNonQuery();
                cs.Close();
                return true;               
            }
        }

        public static List<VaultItem> GetAllItems()
        {
            // todo: refactor to only get items for specific user (given as parameter)
            // todo: sanitize input 

            // todo: decrypt item name before returning it (have a different key to decrypt name than to decrypt credentials)

            List<VaultItem> VaultItemList = new List<VaultItem>();

            using (SqlConnection cs = new SqlConnection(connectionString))
            {                
                    SqlCommand cmd = new SqlCommand("SELECT * FROM VaultItem", cs);                    
                    cs.Open();
                    SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    VaultItemList = DataReaderMapToList<VaultItem>(sqlDataReader);
                    cs.Close();           
            }
            return VaultItemList;
        }

        private static List<T> DataReaderMapToList<T>(SqlDataReader sqlDataReader)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (sqlDataReader.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(sqlDataReader[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, sqlDataReader[prop.Name], null);
                        if (prop.Name.Equals("Password"))
                        {
                            Console.WriteLine();
                            prop.SetValue(obj, Decrypt(Convert.FromBase64String((String)sqlDataReader[prop.Name]), aes_key, IV_aes), null);
                        }
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        internal static bool AuthenticateUser(string userNameInput, string masterPasswordInput)
        {
            // todo: sanitize inputs

            // todo: fix bug where login is not possible because a different hash is generated by the CreateHash() function everytime
            string masterPasswordHash = Convert.ToBase64String(PasswordManagerController.CreateHash(masterPasswordInput));
            
            List<User> AuthenticatedUser = new List<User>();
            using (SqlConnection cs = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM MasterPassword WHERE UserName = @Username AND masterPassword = @MasterPassword", cs);
                cmd.Parameters.AddWithValue("@Username", userNameInput);
                cmd.Parameters.AddWithValue("@MasterPassword", masterPasswordHash);
                cs.Open();
                SqlDataReader sqlDataReader = cmd.ExecuteReader();
                AuthenticatedUser = DataReaderMapToList<User>(sqlDataReader);
                cs.Close();

                if (AuthenticatedUser.Count > 0)
                {
                    return true;
                };
                
                return false;
                
            }
        }

        public static VaultItem GetItem(int idToRetrieve)
        {
            // todo: check/sanitize inputs

            // todo: decrypt item before returning it (decrypt name using one key and credentials using another key)

            List<VaultItem> VaultItemList = new List<VaultItem>();

            using (SqlConnection cs = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM VaultItem WHERE id = @id", cs);
                cmd.Parameters.AddWithValue("@id", idToRetrieve);
                cs.Open();
                SqlDataReader sqlDataReader = cmd.ExecuteReader();
                VaultItemList = DataReaderMapToList<VaultItem>(sqlDataReader);
                cs.Close();
            }
            return VaultItemList[0];            
        }

        static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV_aes);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }

        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}
