namespace SteamAPI
{
    public static class SessionManager
    {
        public static Dictionary<string, AccountSession> Sessions = new Dictionary<string, AccountSession>();

        public static void LoadAllAccounts()
        {
            var accounts = AccountManager.LoadAccounts();
            foreach (var account in accounts)
            {

                Sessions[account.Username] = new AccountSession(account);

            }

        }

        public static void ActiveAccount(string username)
        {
            if (Sessions.ContainsKey(username))
            {
                Sessions[username].Connect();
            }
            else
            {
                Console.WriteLine($"Account {username} not found in session.");
            }
        }

    }
}
