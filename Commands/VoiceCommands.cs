using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MusicalBot.YTtools;
using DSharpPlus.VoiceNext;
using DSharpPlus.Entities;
using MusicalBot.Tools;


namespace MusicalBot.Commands
{
    public class VoiceCommands : BaseCommandModule
    {
        private CancellationTokenSource _cts = new();
        private PlayingTools _pt = new();
        
        [Command("play")]
        [Aliases("p", "toca")]
        public async Task MusicDownload(CommandContext context, params string[] args)
        {
            if (context.Member.VoiceState == null)
            {
                await context.RespondAsync("Entre em um canal de voz");
                return;
            }

            string msg = "";

            foreach (string arg in args)
            {
                msg += arg + " ";
            }

            msg = msg.Trim();

            Bot.Filas[context.Guild.Id].Add(msg);
            await context.Channel.SendMessageAsync($"adicionando {msg} á fila");
            
            DiscordChannel channel = context.Member.VoiceState.Channel;
            if (!channel.Users.Contains(context.Client.CurrentUser))
            {
                await channel.ConnectAsync();
            }
            var vnext = context.Client.GetVoiceNext();
            var connection = vnext.GetConnection(context.Guild) ?? vnext.GetConnection(context.Member.Guild);

            ulong serverId = connection.TargetChannel.GuildId ?? context.Member.VoiceState.Channel.Id;

            if (Bot.Filas[serverId].Count() < 1 || !connection.IsPlaying)
            {

                while (Bot.Filas[context.Guild.Id].Count() > 0)
                {
                    await YtTools.Download(Bot.Filas[context.Guild.Id][0]);

                    string path = _pt.getPath(context);

                    await _pt.PlayAudioAsync(context, connection, path, Bot.Filas[context.Guild.Id][0], _cts.Token);
                    _cts.Cancel();
                    Bot.Filas[context.Guild.Id].RemoveAt(0);
                    File.Delete(path);
                }
            }
            return;
        }

        [Command("sai")]
        [Aliases("quit", "leave")]
        public async Task Sai(CommandContext context)
        {
            string path = _pt.getPath(context);

            var vnext = context.Client.GetVoiceNext();
            var connection = vnext.GetConnection(context.Guild);
            
            connection.Disconnect();

            File.Delete(path);
            Bot.Filas[context.Guild.Id].Clear();
            _cts.Cancel();
            return;
        }

        [Command("pula")]
        [Aliases("next")]
        public async Task Pula(CommandContext context, int removes = 1)
        {
            var vnext = context.Client.GetVoiceNext();
            var connection = vnext.GetConnection(context.Guild);
            var channel = connection.TargetChannel;

            if (context.Member.VoiceState == null)
            {
                await context.RespondAsync("Entre em um canal de voz");
                return;
            }
            if(connection.IsPlaying && connection.TargetChannel == context.Member.VoiceState.Channel)
            {
                string path = _pt.getPath(context);

                try
                {
                    connection.Disconnect();
                }
                catch (Exception ex) { await context.Channel.SendMessageAsync(ex.Message); }

                _cts.Cancel();
                File.Delete(path);
                for(int i = 0; i < removes; i++)
                {
                    Bot.Filas[context.Guild.Id].RemoveAt(0);
                }
            }

            await channel.ConnectAsync();
            vnext = context.Client.GetVoiceNext();
            connection = vnext.GetConnection(context.Guild);

            while ((Bot.Filas[context.Guild.Id].Count() > 0))
            {

                await YtTools.Download(Bot.Filas[context.Guild.Id][0]);

                string path = _pt.getPath(context);
                try
                {
                    await _pt.PlayAudioAsync(context, connection, path, Bot.Filas[context.Guild.Id][0], _cts.Token);
                }
                catch (Exception ex) { await context.Channel.SendMessageAsync(ex.Message); }

                _cts.Cancel();
                Bot.Filas[context.Guild.Id].RemoveAt(0);
                File.Delete(path);
            }
            return;
        }

        [Command("fila")]
        [Aliases("queue", "list", "lista", "f")]
        private async Task teste(CommandContext context)
        {
            string queue = "Essas são os pedidos na fila:\n";
            foreach (string music in Bot.Filas[context.Guild.Id])
            {
                queue += $"{Bot.Filas[context.Guild.Id].IndexOf(music) + 1} - {music}\n";
            }

            await context.Channel.SendMessageAsync(queue);
        }
    }
}


