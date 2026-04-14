using System.Text.Json;

namespace SteamAPI
{
    public static class AccountManager
    {
        private const string AccountsFilePath = "accounts.json";

        public static List<Account> LoadAccounts()
        {
            if (!File.Exists(AccountsFilePath))
            {
                return new List<Account>();
            }
            string json = File.ReadAllText(AccountsFilePath);
            return JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
        }
        public static void SaveAccounts(List<Account> accounts)
        {
            string json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AccountsFilePath, json);
        }
        public static void AddOrUpdateAccount(Account account)
        {
            var accounts = LoadAccounts();
            var existingAccount = accounts.Find(a => a.Username.Equals(account.Username, StringComparison.OrdinalIgnoreCase));
            if (existingAccount != null)
            {
                existingAccount.Password = account.Password;
                existingAccount.SharedSecret = account.SharedSecret;
            }
            else
            {
                accounts.Add(account);
            }
            SaveAccounts(accounts);
        }
    }
}
