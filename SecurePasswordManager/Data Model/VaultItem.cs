using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Classes
{
    internal class VaultItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public VaultItem(string name, string userName, string password)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            
        }
        public VaultItem()
        {
            
        }

        public override string ToString()
        {
            return "Id: " + Id + ", Name: " + Name + ", Username: " + UserName + ", Password: " + Password;
        }
        
    }
}

