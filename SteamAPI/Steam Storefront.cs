using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SteamAPI
{
    class SteamStorefront
    {
        static async Task Main(string[] args)
        {
            // ID игры, например CS:GO = 730
            int appId = 548430;

            string url = $"https://store.steampowered.com/api/appdetails?appids={appId}&cc=US&l=en";

            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            // Преобразуем JSON в объект
            var json = JsonConvert.DeserializeObject<JObject>(response);

            // Достаём данные по appId
            var appData = json[appId.ToString()]["data"];

            if (appData == null)
            {
                Console.WriteLine("Игра не найдена или API не вернул данные.");
                return;
            }

            // Выводим основные данные
            Console.WriteLine("=== Информация о игре ===");
            Console.WriteLine($"Название: {appData["name"]}");
            Console.WriteLine($"Описание: {appData["short_description"]}");
            Console.WriteLine($"Разработчик: {string.Join(", ", appData["developers"] ?? new JArray())}");
            Console.WriteLine($"Издатель: {string.Join(", ", appData["publishers"] ?? new JArray())}");
            Console.WriteLine($"Дата релиза: {appData["release_date"]?["date"]}");
            Console.WriteLine($"Жанры: {string.Join(", ", appData["genres"]?.Select(g => g["description"].ToString()) ?? new string[0])}");
            Console.WriteLine($"Платформы: Windows: {appData["platforms"]?["windows"]}, Mac: {appData["platforms"]?["mac"]}, Linux: {appData["platforms"]?["linux"]}");
            Console.WriteLine($"Цена: {appData["price_overview"]?["final_formatted"] ?? "Бесплатно или недоступно"}");

            if (appData["achievements"] != null)
            {
                Console.WriteLine("\nДостижения:");
                foreach (var ach in appData["achievements"])
                {
                    Console.WriteLine($"- {ach["name"]} : {ach["description"]}");
                }
            }

            // DLC (если есть)
            if (appData["dlc"] != null)
            {
                Console.WriteLine("\nDLC:");
                foreach (var dlcId in appData["dlc"])
                {
                    Console.WriteLine($"- {dlcId}");
                }
            }

            Console.WriteLine("\n=== Конец информации ===");
        }
    }
}
