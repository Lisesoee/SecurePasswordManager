using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurePasswordManager.Classes;
using SecurePasswordManager.Core;



namespace SecurePasswordManager.UI
{
    internal class PasswordManagerUI
    {
        public PasswordManagerUI()
        {
        }

        public static void RunSecurePasswordManager()
        {
            if (LoginMenu())
            {
                MainMenu();
            }
        }

        private static bool LoginMenu()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("1: Login using existing user");
            Console.WriteLine("2: Create new user");
            Console.WriteLine("3: Forgot password");
            Console.WriteLine("4: Exit menu");
            Console.WriteLine("Please select an option: ");

            bool exit = false;
            while (exit == false)
            {
                switch (Console.ReadLine())
                {
                    case "1":
                        if (LoginMenuOption_Login()){
                            return true; 
                        }
                        break;

                    case "2":
                        LoginMenuOption_CreateNewUser();
                        break;
                    case "3":
                        LoginMenuOption_ForgotPassword();
                        break;
                    case "4":
                        exit = true;
                        break;
                }
            }

            return false;
        }

        private static void LoginMenuOption_ForgotPassword()
        {
            throw new NotImplementedException();
        }

        private static void LoginMenuOption_CreateNewUser()
        {
            Console.WriteLine("Please input a user name:");
            string userNameInput = Console.ReadLine();
            Console.WriteLine("Please input a master password:");
            string masterPasswordInput = Console.ReadLine();
            Console.WriteLine("Please verify master password:");
            string masterPasswordInput2 = Console.ReadLine();

            if (masterPasswordInput != masterPasswordInput2)
            {
                throw new Exception("The two input passwords don't match!");
            }

            if (PasswordManagerController.CreateNewUser(userNameInput, masterPasswordInput))
            {
                Console.WriteLine("User was successfully created!");
            }
            else
            {
                Console.WriteLine("The user was not created. Please try again.");
            }
        }

        private static bool LoginMenuOption_Login()
        {
            return true; // for testing
            throw new NotImplementedException();
        }
                

        private static void MainMenu()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("1: Add new Item");
            Console.WriteLine("2: Retrieve Vault Item");
            Console.WriteLine("3: Exit menu");
            Console.WriteLine("Please select an option: ");

            bool exit = false;
            while (exit == false)
            {
                switch (Console.ReadLine())
                {
                    case "1":
                        MenuOption_AddNewItem();
                        break;
                    case "2":
                        MenuOption_RetrieveVaultItem();
                        break;
                    case "3":
                        exit = true;
                        break;
                }
            }
        }

        private static void MenuOption_RetrieveVaultItem()
        {
            List<VaultItem> VaultItemList = new List<VaultItem>();

            try
            {
                VaultItemList = PasswordManagerController.GetAllItems();
            }
            catch
            {
                Console.WriteLine("The existing items could not be retrieved from the vault.");
            };
            

            foreach (VaultItem item in VaultItemList)
            {
                Console.WriteLine("Id: " + item.Id + ", Name: " + item.Name);
            }

            Console.WriteLine("Which item would you like to retrieve the password for?: ");
            int idToRetrieve = int.Parse(Console.ReadLine());

            VaultItem vaultItem = PasswordManagerController.GetItem(idToRetrieve);
            Console.WriteLine(vaultItem.ToString());

        }

        private static void MenuOption_AddNewItem()
        {
            Console.WriteLine("Please input a name: ");
            string name = Console.ReadLine();
            Console.WriteLine("Please input a username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Please input a password: ");
            string password = Console.ReadLine();

            if (PasswordManagerController.CreateNewItem(name, username, password))
            {
                Console.WriteLine("The item was created successfully!");
            }
            else
            {
                Console.WriteLine("The item could not be created.");
            }          
            
        }
        
    }
}
