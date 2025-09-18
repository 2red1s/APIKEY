using SteamKit2;
using SteamKit2.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2.Internal;
//using Steamworks;

namespace SteamAPI
{
    class RedySoft {
        
        static SteamClient steamClient;
        static CallbackManager manager;
        static SteamUser steamUser;
        static SteamFriends steamFriends;

        static string lastLogin;
        static string lastPassword;
        static string authCode;


        static void Main(string[] args) {
            steamClient = new SteamClient();
            manager = new CallbackManager(steamClient);
            steamUser = steamClient.GetHandler<SteamUser>();
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            steamFriends = steamClient.GetHandler<SteamFriends>();

            steamClient.Connect();
            
            Console.WriteLine("Connecting to Steam...");

            while(true)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Enter your Steam username:");
            lastLogin = Console.ReadLine();
            Console.WriteLine("Enter your Steam password:");
            lastPassword = Console.ReadLine();
            


            Console.WriteLine("Connected to Steam!");



            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = lastLogin,
                Password = lastPassword
            });

        }



        static void OnLoggedOn(SteamUser.LoggedOnCallback loggedOnCallback) {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            
            if (loggedOnCallback.Result == EResult.AccountLogonDenied)
            {
                Console.WriteLine("Please write code from Mail");
                authCode = Console.ReadLine();

                steamUser.LogOn(new SteamUser.LogOnDetails
                {   
                    Username = lastLogin,
                    Password = lastPassword,
                    AuthCode = authCode
                });

            }
            else if (loggedOnCallback.Result == EResult.AccountLoginDeniedNeedTwoFactor)
            {
                Console.WriteLine("Please write code from Steam Guard Mobile Authenticator");
                authCode = Console.ReadLine();

                steamUser.LogOn(new SteamUser.LogOnDetails
                {
                    Username = lastLogin,
                    Password = lastPassword,
                    TwoFactorCode = authCode
                });
            }
            else if (loggedOnCallback.Result == EResult.OK)
            {
                Console.WriteLine("Success Logged to Account");
                StartGame(730, "CS:GO");

                Console.WriteLine("Press Enter to stop playing the game...");
                Console.ReadLine();
                StopGame();
            }
            else
            {
                Console.WriteLine("LoggedOn Result: " + loggedOnCallback.Result);
            }
            

        }

        static void StartGame(int appId, string gameName = null)
        {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playingGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = 730, // Replace with the desired game's AppID
                game_extra_info = gameName ?? $"Playing Game {appId}"
            });

            steamClient.Send(playingGame);

            Console.WriteLine($"Started playing game with AppID: {appId}");
        }

        static void StopGame()
        {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            // Sending an empty list indicates that the user has stopped playing any game
            steamClient.Send(playingGame);
            Console.WriteLine("Stopped playing game.");
        }



    }


}

