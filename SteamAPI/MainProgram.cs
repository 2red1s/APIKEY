using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;



namespace SteamAPI
{
    public class MainProgram
    {
        static async Task Main()
        {

            //GetNewsForApp (v0002)

            string newsforapp = $"{Headers.BaseURL}ISteamNews/GetNewsForApp/v0002/?appid=730&count=3&maxlength=300&format=json\r\nArguments";
            Console.WriteLine(await GetApiresponse(newsforapp));

            Console.WriteLine("-------------------------------------------------");

            //Полная информация по аккаунту
            string accurl = $"{Headers.BaseURL}ISteamUser/GetPlayerSummaries/v0002/?key={Headers.APIKey}&steamids={Headers.SteamID}";
            Console.WriteLine(await GetApiresponse(accurl));

            Console.WriteLine("-------------------------------------------------");

            //получить список друзей
            string friendurl = $"{Headers.BaseURL}ISteamUser/GetFriendList/v0001/?key={Headers.APIKey}&steamid={Headers.SteamID}&relationship=friend";
            Console.WriteLine(await GetApiresponse(friendurl));

            //GetPlayerAchievements (v0001) получить список ачивок об игре
            Console.WriteLine("---------------PlayerAchiev001----------------------------------");

            //GetPlayerAchievements (v0001)
            string playerachiev = $"{Headers.BaseURL}ISteamUserStats/GetPlayerAchievements/v0001/?appid=548430&key={Headers.APIKey}&steamid={Headers.SteamID}";
            Console.WriteLine(await GetApiresponse(playerachiev));

            //GetUserStatsForGame (v0002)
            Console.WriteLine("-----------------------GetUserStatsForGamev0002--------------------------");

            string userstatforgames = $"{Headers.BaseURL}ISteamUserStats/GetUserStatsForGame/v0002/?appid=548430&key={Headers.APIKey}&steamid={Headers.SteamID}";
            Console.WriteLine(await GetApiresponse(userstatforgames));

            Console.WriteLine("-------------------------------------------------");


            //GetRecentlyPlayedGames(v0001) последние 3 игры которые запускал (информация по ним)
            Console.WriteLine("-----------------------GetRecentlyPlayedGamesv0001--------------------------");
            string recentlyplayedgames = $"{Headers.BaseURL}IPlayerService/GetRecentlyPlayedGames/v0001/?key={Headers.APIKey}&steamid={Headers.SteamID}&format=json";
            Console.WriteLine(await GetApiresponse(recentlyplayedgames));



            //GetOwnedGames (v0001)
            Console.WriteLine("\n--------------------GetOwnedGamesv0001-----------------------------");
            string achievurl = $"{Headers.BaseURL}IPlayerService/GetOwnedGames/v0001/?key={Headers.APIKey}&steamid={Headers.SteamID}&include_appinfo=true&include_played_free_games=true&format=json\r\n";
            Console.WriteLine(await GetApiresponse(achievurl));


            //парсинг определенных данных
            string gameresponse = await GetApiresponse(achievurl);
            using JsonDocument gamedoc = JsonDocument.Parse(gameresponse);

            int gameCount = gamedoc.RootElement
                .GetProperty("response")
                .GetProperty("game_count")
                .GetInt32();
            Console.WriteLine("Количество игр:" + gameCount);

            JsonElement games = gamedoc.RootElement
                .GetProperty("response")
                .GetProperty("games");
            int playtime = games[3].GetProperty("playtime_forever").GetInt32();
            string firstgames = games[3].GetProperty("name").GetString();
            Console.WriteLine("Первая игра:" + firstgames + " | часов в игре " + (playtime / 60));


            //парсинг профиля
            string profileresponse = await GetApiresponse(accurl);
            using JsonDocument profiledoc = JsonDocument.Parse (profileresponse);

            JsonElement profilename = profiledoc.RootElement
                .GetProperty("response")
                .GetProperty("players");
            var firstplayer = profilename[0];     
            string personalname = firstplayer.GetProperty("personaname").GetString();

            string personallinks = firstplayer.GetProperty("profileurl").GetString();

            string personalcountry = firstplayer.GetProperty("loccountrycode").GetString();

            Console.WriteLine("Name:" + personalname + " | " + personallinks + " | " + personalcountry);
        }

        static async Task<string> GetApiresponse(string url)
        {

            using HttpClient client = new HttpClient();
            return await client.GetStringAsync(url);

        }


    }
}
