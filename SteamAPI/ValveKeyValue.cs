using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ValveKeyValue;

namespace SteamAPI
{
    class ValveKeyValue
    {
        public static void Run()
        {
            string path = @"C:\Program Files (x86)\Steam\config\loginusers.vdf";

            using (var stream = File.OpenRead(path))
            {
                var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Deserialize(stream);
                foreach (var user in kv.Children)
                {
                    string steamID = user.Name;
                    string accountName = user["AccountName"]?.ToString() ?? "N/A";
                    string personaName = user["PersonaName"]?.ToString() ?? "N/A";

                    Console.WriteLine($"PersonaName: {personaName}");
                    Console.WriteLine($"AccountName: {accountName}");
                    Console.WriteLine($"SteamID: {steamID}");
                    Console.WriteLine(new string('-', 40));
                }
            }
        }
    }
}
