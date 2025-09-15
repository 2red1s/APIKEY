using SteamKit2;
using SteamKit2.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2.Internal;

namespace SteamAPI
{
    class RedySoft {
        static SteamClient steamClient;
        static CallbackManager manager;
        static SteamUser steamUser;
        static SteamFriends steamFriends;

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
            string userlogin = Console.ReadLine();
            Console.WriteLine("Enter your Steam password:");
            string userpassword = Console.ReadLine();
            Console.WriteLine("Enter your 2FA code:");
            string shouldPassword = Console.ReadLine();


            Console.WriteLine("Connected to Steam!");

            

            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = userlogin,
                Password = userpassword,
                //TwoFactorCode = "12345", // If you have 2FA enabled, provide the code here
                TwoFactorCode = shouldPassword,
            });
            //var authSession = await steamClient.Authentication.BeginAuthSessionViaCredentialsAsync(new AuthSessionDetails 
            //{
            //    Username = "lowstea__m",
            //    Password = "78810406Artik",
            //    IsPersistentSession = shouldRememberPassword,

            //});




        }



        static void OnLoggedOn(SteamUser.LoggedOnCallback loggedOnCallback) {
            var playingGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            if (loggedOnCallback.Result == EResult.OK)
            {
                Console.WriteLine("Success Logged to Account");
                
                

            }
            else
            {
                Console.WriteLine("LoggedOn Result: " + loggedOnCallback.Result);
            }
            

        }

         

    }


}

