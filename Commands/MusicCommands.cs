using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MusicalBot.YTtools;
using DSharpPlus.VoiceNext;
using DSharpPlus.Entities;
using MusicalBot.Tools;
using Microsoft.VisualBasic;


namespace MusicalBot.Commands
{
    public class MusicCommands : BaseCommandModule
    {
        private CancellationTokenSource _cts = new();
        private PlayingTools _pt = new();

        [Command("play")]
        [Aliases("p", "toca")]
        public async Task MusicDownload(CommandContext context, params string[] args)
        {
            if(context.Member == null || Bot.Filas == null )
            {
                return;
            }
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

            await _pt.AddToQueue(context, msg);

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
                    await YtTools.Download(Bot.Filas[context.Guild.Id][0][0]);

                    string path = _pt.getPath(context) ?? "";

                    await _pt.PlayAudioAsync(context, connection, path, Bot.Filas[context.Guild.Id][0][1], _cts.Token);
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
            if(Bot.Filas == null )
            {
                return;
            }
            string path = _pt.getPath(context) ?? "";

            var vnext = context.Client.GetVoiceNext();
            var connection = vnext.GetConnection(context.Guild);

            connection.Disconnect();

            File.Delete(path);
            Bot.Filas[context.Guild.Id].Clear();
            _cts.Cancel();

            await context.Channel.SendMessageAsync("Obrigado por usar o bot!");
            return;
        }

        [Command("pula")]
        [Aliases("next")]
        public async Task Pula(CommandContext context, int removes = 1)
        {
            if(context.Member == null || Bot.Filas == null )
            {
                return;
            }

            var vnext = context.Client.GetVoiceNext();
            var connection = vnext.GetConnection(context.Guild);

            if (connection == null)
            {
                await context.RespondAsync("Não estou tocando nada");
                return;
            }
            var channel = connection.TargetChannel;
            if (Bot.Filas[context.Guild.Id].Count < removes)
            {
                await context.RespondAsync("Não tem tantas faixas para serem puladas");
                return;
            }
            if (context.Member.VoiceState == null)
            {
                await context.RespondAsync("Entre em um canal de voz");
                return;
            }
            if (connection.IsPlaying && connection.TargetChannel == context.Member.VoiceState.Channel)
            {
                string path = _pt.getPath(context) ?? "";

                try
                {
                    connection.Disconnect();
                }
                catch (Exception ex) { await context.Channel.SendMessageAsync(ex.Message); }

                _cts.Cancel();
                File.Delete(path);
                for (int i = 0; i < removes; i++)
                {
                    Bot.Filas[context.Guild.Id].RemoveAt(0);
                }
            }

            await channel.ConnectAsync();
            vnext = context.Client.GetVoiceNext();
            connection = vnext.GetConnection(context.Guild);

            while (Bot.Filas[context.Guild.Id].Count() > 0)
            {
                await YtTools.Download(Bot.Filas[context.Guild.Id][0][0]);

                string path = _pt.getPath(context) ?? "";

                await _pt.PlayAudioAsync(context, connection, path, Bot.Filas[context.Guild.Id][0][1], _cts.Token);
                _cts.Cancel();
                Bot.Filas[context.Guild.Id].RemoveAt(0);
                File.Delete(path);
            }
            return;
        }

        [Command("tira")]
        [Aliases("remove", "retira", "rm")]
        public async Task Tira(CommandContext context, int index)
        {
            if(Bot.Filas == null)
            {
                return;
            }
            
            if (index <= 1)
            {
                await context.Channel.SendMessageAsync("Para pular a musica que esta tocando use o comando Pula");
                return;
            }
            if (index > Bot.Filas[context.Guild.Id].Count())
            {
                await context.Channel.SendMessageAsync("Verifque a posição correta da musica na fila");
                return;
            }
            await context.Channel.SendMessageAsync($"{Bot.Filas[context.Guild.Id][index - 1][1]} removido da fila");
            Bot.Filas[context.Guild.Id].RemoveAt(index - 1);
        }

        [Command("adiciona")]
        [Aliases("add", "put")]
        public async Task Add(CommandContext context, int index = 0, params string[] args)
        {
            string msg = "";

            foreach (string arg in args)
            { 
                msg += arg + " ";
            }

            msg = msg.Trim();

            if (msg.Contains("list="))
            {
                await context.RespondAsync("Não é possivel adicionar playlists com esse comando");
                return;
            }
            if (index == 1 || index == 0)
            {
                await context.RespondAsync("Não é possivel pular musicas com esse comando");
                return;
            }
            await _pt.AddToQueue(context, msg, index -1);
        }

    }
}


