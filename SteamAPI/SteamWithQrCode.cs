using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using SteamKit2.Authentication;


namespace SteamAPI
{   
    class SteamWithQrCode
    {
        static SteamClient steamClient;
        static CallbackManager manager;
        static SteamUser steamUser;
        void Main(string[] args)
        {
            var steamClient = new SteamClient();
            var manager = new CallbackManager(steamClient);
            var steamUser = steamClient.GetHandler<SteamUser>();

            manager.Subcribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subcribe<SteamUser.LoggedOnCallback>(OnLoggedOn);


            steamClient.Connect();

            Console.WriteLine("Connecting to Steam...");

            while (true)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }
    }
}
