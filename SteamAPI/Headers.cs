using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
namespace SteamAPI
{
    public static class Headers
    {
        public static string SteamID {  get; set; } = "76561199120202673";
        public static string BaseURL { get; } = "http://api.steampowered.com/";

        public static string APIKey { get; }

        static Headers()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            APIKey = config["SteamAPI:Key"];
        }



    }





}
