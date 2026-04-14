using SteamKit2;
using SteamKit2.Internal;


namespace SteamAPI
{
    class RedySoft
    {
        static SteamClient steamClient;
        static CallbackManager manager;
        static SteamUser steamUser;
        static SteamFriends steamFriends;

        static string lastLogin;
        static string lastPassword;
        static string pendingAuthCode = null;
        static string savedLoginKey = null;

        static bool needEmailCode = false;
        static bool needTwoFactor = false;
        static bool isLoggingIn = false;

        static void Main(string[] args)
        {
            steamClient = new SteamClient();
            manager = new CallbackManager(steamClient);
            steamUser = steamClient.GetHandler<SteamUser>();
            steamFriends = steamClient.GetHandler<SteamFriends>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);


            Console.WriteLine("Connecting to Steam...");
            steamClient.Connect();

            while (true)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected to Steam!");

            if (!isLoggingIn)
            {
                Console.WriteLine("Enter your Steam username:");
                lastLogin = Console.ReadLine();
                Console.WriteLine("Enter your Steam password:");
                lastPassword = Console.ReadLine();
            }

            PerformLogin();
        }

        static void PerformLogin()
        {
            isLoggingIn = true;

            var logOnDetails = new SteamUser.LogOnDetails
            {
                Username = lastLogin,
                Password = lastPassword
            };

            if (needEmailCode && !string.IsNullOrEmpty(pendingAuthCode))
            {
                logOnDetails.AuthCode = pendingAuthCode;
            }
            else if (needTwoFactor && !string.IsNullOrEmpty(pendingAuthCode))
            {
                logOnDetails.TwoFactorCode = pendingAuthCode;
            }

            steamUser.LogOn(logOnDetails);
        }

        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.AccountLogonDenied)
            {
                Console.WriteLine("Write code from Email:");
                pendingAuthCode = Console.ReadLine();
                needEmailCode = true;
                needTwoFactor = false;
                steamClient.Disconnect();
                return;

            }

            if (callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
            {
                Console.WriteLine("Write code from Mobile Guard:");
                pendingAuthCode = Console.ReadLine();
                needTwoFactor = true;
                needEmailCode = false;
                steamClient.Disconnect();
                return;
            }

            if (callback.Result == EResult.OK)
            {
                Console.WriteLine("Logged in successfully: " + callback.Result);
                StartGame(515570, "game"); // idgame
                Console.WriteLine("Write to stop playing...");
                Console.ReadLine();
                StopGame();
                return;
            }

            Console.WriteLine($"Login failed: {callback.Result}");
            isLoggingIn = false;
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected from Steam");

            if (needEmailCode || needTwoFactor)
            {
                Thread.Sleep(2000);
                steamClient.Connect();
            }
        }

        static void StartGame(int appId, string gameName = null)
        {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            playingGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = (ulong)appId,
                game_extra_info = gameName ?? $"Playing Game {appId}"
            });
            steamClient.Send(playingGame);
            Console.WriteLine($"Started playing game with AppID: {appId}");
        }

        static void StopGame()
        {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            steamClient.Send(playingGame);
            Console.WriteLine("Stopped playing game.");

        }
    }
}