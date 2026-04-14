using System;

namespace SteamAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            SessionManager.LoadAllAccounts();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Steam Account Manager ===");
                Console.WriteLine("1. Add new account");
                Console.WriteLine("2. Show all accounts");
                Console.WriteLine("3. Activate account");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddAccount();
                        break;
                    case "2":
                        ShowAccounts();
                        break;
                    case "3":
                        ActivateAccount();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static async Task AddAccount()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            Console.Write("Enter Mobile 2FA secret (optional, press Enter to skip): ");
            string sharedSecret = Console.ReadLine();

            var account = new Account
            {
                Username = username,
                Password = password,
                SharedSecret = string.IsNullOrEmpty(sharedSecret) ? null : sharedSecret
            };


            var loginHandler = new AccountLoginHandler(account);

            //var loginCompletionSource = new TaskCompletionSource<bool>();
            //loginHandler.OnLoginSuccess += (handler) =>
            //{
            //    Console.WriteLine($"Account {account.Username} logged is successfully.");
            //    loginCompletionSource.SetResult(true);

            //};

            //loginHandler.OnCodeNeeded += () =>
            //{
            //    Console.Write("Enter Steam Guard Code (Email or 2FA): ");
            //    string code = Console.ReadLine();
            //    loginHandler.SetAuthCode(code);
            //};

            //loginHandler.Login();

           // await loginCompletionSource.Task;

            //while (!loginCompleted)
            //{
            //    Thread.Sleep(500);
            //    if (loginHandler != null && (loginHandler.WaitingForCode)) continue;
            //}

            AccountManager.AddOrUpdateAccount(account);
            SessionManager.Sessions[username] = new AccountSession(account);

            Console.WriteLine($"Account {username} added. Press Enter to continue...");
            Console.ReadLine();
        }


        static void ActivateAccount()
        {
            Console.Write("Enter username to activate: ");
            string username = Console.ReadLine();

            if (SessionManager.Sessions.ContainsKey(username))
            {
                SessionManager.Sessions[username].Connect();
                Console.WriteLine($"Account {username} activated. Press Enter to continue...");
            }
            else
            {
                Console.WriteLine("Account not found in session. Press Enter to continue...");
            }
            Console.ReadLine();
        }
        static void ShowAccounts()
        {
            var accounts = AccountManager.LoadAccounts();
            if (accounts.Count == 0)
            {
                Console.WriteLine("No accounts found.");
            }
            else
            {
                Console.WriteLine("Saved accounts:");
                foreach (var acc in accounts)
                    Console.WriteLine($"- {acc.Username}");
            }
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
