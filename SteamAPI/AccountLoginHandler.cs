
using SteamKit2;
using SteamKit2.Internal;

namespace SteamAPI
{
    public class AccountLoginHandler
    {
        private SteamClient steamClient;
        private CallbackManager manager;
        private SteamUser steamUser;
        private SteamFriends steamFriends;

        private Account account;
        private string pendingAuthCode = null;
        private bool needEmailCode = false;
        private bool needTwoFactor = false;
        private bool isLoggingIn = false;

        public AccountLoginHandler(Account acc)
        {
            account = acc;
            steamClient = new SteamClient();
            manager = new CallbackManager(steamClient);
            steamUser = steamClient.GetHandler<SteamUser>();
            steamFriends = steamClient.GetHandler<SteamFriends>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            new Thread(() =>
            {
                while (true)
                {
                    manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                    Thread.Sleep(10);
                }
            }).Start();
        }

        public void Login()
        {
            if (isLoggingIn) return;
            steamClient.Connect();
        }

        private void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected to Steam!");

            if (!isLoggingIn)
            {
                if (string.IsNullOrEmpty(account.Username))
                {
                    Console.WriteLine("Enter your Steam username:");
                    account.Username = Console.ReadLine();
                }

                if (string.IsNullOrEmpty(account.Password))
                {
                    Console.WriteLine("Enter your Steam password:");
                    account.Password = Console.ReadLine();
                }
            }

            PerformLogin();
        }

        private void PerformLogin()
        {
            isLoggingIn = true;

            var logOnDetails = new SteamUser.LogOnDetails
            {
                Username = account.Username,
                Password = account.Password
            };

            if (needEmailCode && !string.IsNullOrEmpty(pendingAuthCode))
                logOnDetails.AuthCode = pendingAuthCode;
            else if (needTwoFactor && !string.IsNullOrEmpty(pendingAuthCode))
                logOnDetails.TwoFactorCode = pendingAuthCode;

            steamUser.LogOn(logOnDetails);
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            switch (callback.Result)
            {
                case EResult.OK:
                    Console.WriteLine("Logged in successfully!");
                    AccountManager.AddOrUpdateAccount(account);

                    isLoggingIn = false;
                    needTwoFactor = false;
                    needEmailCode = false;
                    pendingAuthCode = null;

                    StartGame(760160, "game");
                    Console.WriteLine("Write to stop playing...");
                    Console.ReadLine();
                    StopGame();
                    return;

                case EResult.AccountLogonDenied:
                    Console.WriteLine("Write code from Email:");
                    pendingAuthCode = Console.ReadLine();
                    needEmailCode = true;
                    needTwoFactor = false;
                    isLoggingIn = false;
                    PerformLogin();
                    return;

                case EResult.AccountLoginDeniedNeedTwoFactor:
                    Console.WriteLine("Write code from Mobile Guard:");
                    pendingAuthCode = Console.ReadLine();
                    needTwoFactor = true;
                    needEmailCode = false;
                    isLoggingIn = false;
                    PerformLogin();
                    return;

                default:
                    Console.WriteLine($"Login failed: {callback.Result}");
                    isLoggingIn = false;
                    break;
            }
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected from Steam");
        }

        public void StartGame(int appId, string gameName = null)
        {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            playingGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = (ulong)appId,
                game_extra_info = gameName ?? $"Playing Game {appId}"
            });
            steamClient.Send(playingGame);
            Console.WriteLine($"Started playing {gameName ?? appId.ToString()}");
        }

        public void StopGame()
        {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            steamClient.Send(playingGame);
            Console.WriteLine("Stopped playing game.");
        }
    }
}

