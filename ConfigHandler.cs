using DSharpPlus;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NovoBot
{
    public class ConfigHandler
    {
        private static string _jsonPath;
        private Dictionary<string, string> _jsonValues;
        public string Token { get; private set; }
        public string Prefix { get; private set; }

        public DiscordConfiguration ClientConfig { get; private set; }
        public CommandsNextConfiguration CommandsConfig { get; private set; }
        public static bool Linux { get; private set; }
        public ConfigHandler()
        {
            Linux = OperatingSystem.IsLinux();

            if (Linux)
            {
                _jsonPath = Directory.GetCurrentDirectory() + "/config.json";
            }
            else 
            {
                _jsonPath = Directory.GetCurrentDirectory() + @"\config.json";
            }


            string json = File.ReadAllText(_jsonPath);
            
            try
            {
                _jsonValues = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            catch (ArgumentNullException e)
            {
                throw new Exception("Json não encontrado" + e.Message);
            }

            Token = _jsonValues["token"];
            Prefix = _jsonValues["prefix"];

            ClientConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = this.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            CommandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new String[] { Prefix },
                CaseSensitive = false,
                EnableDms = true,
                EnableMentionPrefix = false,
                EnableDefaultHelp = false,
                
            };
        }
    }
}
