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
        private static string? _jsonPath;
        private Dictionary<string, string>? _jsonValues;
        public string? Token { get; private set; }
        public static string? Prefix { get; private set; }
        public DiscordConfiguration? ClientConfig { get; private set; }
        public CommandsNextConfiguration? CommandsConfig { get; private set; }
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

            VerifyOrCreateJson();
            VerifyOrCreateMusicsDir();

            string json = File.ReadAllText(_jsonPath);
            
            _jsonValues = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            
            if(_jsonValues == null)
            {
                return;
            }

            Token = _jsonValues["token"];
            Prefix = _jsonValues["prefix"];
            try
            {
                ClientConfig = new DiscordConfiguration()
                {
                    Intents = DiscordIntents.All,
                    Token = this.Token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("O token não esta preenchido ou esta incorreto " + e.Message);
                Console.ReadKey();
            }
            CommandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new String[] { Prefix },
                CaseSensitive = false,
                EnableDms = true,
                EnableMentionPrefix = false,
                EnableDefaultHelp = false,
            };
        }

        private void VerifyOrCreateJson()
        {
            if (!File.Exists(_jsonPath))
            {
                var jsonValues = new Dictionary<string, string>();
                jsonValues.Add("prefix", "$");
                jsonValues.Add("token", "");
                jsonValues.Add("email", "");
                jsonValues.Add("password", "");
                jsonValues.Add("userName", "");
                jsonValues.Add("invite", "");

                var contnet = JsonSerializer.Serialize(jsonValues);

                using( var writer = new StreamWriter(_jsonPath ?? "./"))
                {
                    writer.Write(contnet);
                }
            }
            return;
        }

        private void VerifyOrCreateMusicsDir()
        {
            string path = Directory.GetCurrentDirectory();
            if (Linux)
            {
                path += "/musics/";
            }
            else 
            {
                path += @"\musics\";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
