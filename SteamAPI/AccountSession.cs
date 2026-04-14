using SteamKit2;
using SteamKit2.Internal;

namespace SteamAPI
{
    public class AccountSession
    {
        public Account Account { get; private set; }
        public SteamClient steamClient { get; private set; }
        public CallbackManager manager { get; private set; }
        public SteamUser steamUser { get; private set; }
        public SteamFriends steamFriends { get; private set; }


        public AccountSession(Account account)
        {
            Account = account;

            steamClient = new SteamClient();
            manager = new CallbackManager(steamClient);
            steamUser = steamClient.GetHandler<SteamUser>();
            steamFriends = steamClient.GetHandler<SteamFriends>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            Task.Run(() =>
            {

                while (true)
                {
                    manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                    Thread.Sleep(10);
                }
            });

        }
        public void Connect()
        {
            steamClient.Connect();
        }

        private void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine($"[{Account.Username}] Connected to Steam.");
            var logOnDetails = new SteamUser.LogOnDetails
            {
                Username = Account.Username,
                Password = Account.Password
            };
            steamUser.LogOn(logOnDetails);
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                Console.WriteLine($"[{Account.Username}] Login successful!");
            }
            else if (callback.Result == EResult.AccountLogonDenied)
            {
                Console.WriteLine($"[{Account.Username}] Email auth needed.");
            }
            else if (callback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
            {
                Console.WriteLine($"[{Account.Username}] Mobile 2FA needed.");
            }
            else
            {
                Console.WriteLine($"[{Account.Username}] Login failed: {callback.Result}");
            }
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine($"[{Account.Username}] Disconnected from Steam.");
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
            Console.WriteLine($"[{Account.Username}] Started playing {gameName ?? appId.ToString()}");
        }

        public void StopGame()
        {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            steamClient.Send(playingGame);
            Console.WriteLine($"[{Account.Username}] Stopped playing game.");
        }
    }
}


