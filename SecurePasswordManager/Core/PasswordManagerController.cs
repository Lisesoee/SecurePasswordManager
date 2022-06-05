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

namespace SecurePasswordManager.Core
{
    internal class PasswordManagerController
    {
        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\jacobsl\OneDrive - Hansen Technologies Limited\Drive\PBA\Courses\Software Sequrity\Exam Project\PasswordManagerProject\SecurePasswordManager\Database\PasswordVault.mdf;Integrated Security=True;Connect Timeout=30";
        
        
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

        

        public static bool CreateNewUser(string userNameInput, string masterPasswordInput)
        {
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
            using (SqlConnection cs = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO VaultItem (Name, UserName, Password) values (@name,@username,@password)", cs);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cs.Open();
                cmd.ExecuteNonQuery();
                cs.Close();
                return true;               
            }
        }

        public static List<VaultItem> GetAllItems()
        {
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
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        public static VaultItem GetItem(int idToRetrieve)
        {
            // decrypt
            throw new NotImplementedException();
            
        }
    }
}
