using DSharpPlus.CommandsNext;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.VoiceNext;
using MusicalBot.Commands;
using NovoBot;

namespace MusicalBot
{
    public static class Bot
    {
        private static DiscordClient? _client { get; set; }
        private static CommandsNextExtension? _commands { get; set; }
        public static Dictionary<ulong, List<string[]>>? Filas { get; set; }

        public static async Task Run()
        {
            ConfigHandler CfHandler = new();

            _client = new DiscordClient(CfHandler.ClientConfig);
            if(CfHandler.CommandsConfig == null)
            {
                return;
            }
            _client.UseVoiceNext();

            Filas = new Dictionary<ulong, List<string[]>>();

            _client.Ready += _client_Ready;
            _commands = _client.UseCommandsNext(CfHandler.CommandsConfig);
            _commands.RegisterCommands<MusicCommands>();
            _commands.RegisterCommands<TextCommands>();

            await _client.ConnectAsync();

            await Task.Delay(-1);
        }

        public async static Task Shutdown()
        { 
            if(_client != null)
            {
                await _client.DisconnectAsync();
                _client.Dispose();    
            }
        }
        private static Task _client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            if(_client == null)
            {
                return Task.CompletedTask;
            }
            foreach(var server in _client.Guilds)
            {
                string musicPath;

                if(Filas == null)
                {
                    return Task.CompletedTask;
                }

                if (!Filas.ContainsKey(server.Value.Id))
                {
                    Filas.Add(server.Value.Id, new List<string[]>());
                }
                
                if (ConfigHandler.Linux)
                {
                    musicPath = "/musics/";
                }
                else { musicPath = @"\musics\"; }

                var toDeleteMusics = Directory.GetFiles(Directory.GetCurrentDirectory() + musicPath);

                foreach (var file in toDeleteMusics)
                {
                    File.Delete(file);
                }
            }

            return Task.CompletedTask;

        }
    }
}
