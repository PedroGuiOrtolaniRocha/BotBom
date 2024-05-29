using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MusicalBot.YTtools;
using NAudio.Wave;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using NovoBot;

namespace MusicalBot.Commands
{
    public class VoiceCommands : BaseCommandModule
    {
        private CancellationTokenSource _cts = new();
        private WaveFormat _waveFormat = new(48000, 16, 2);

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
            string musicPath; 

            foreach (string arg in args)
            {
                msg += arg + " ";
            }

            Bot.Filas[context.Guild.Id].Add(msg);
            await context.Channel.SendMessageAsync($"adicionando {msg} á fila");
            
            DiscordChannel channel = context.Member.VoiceState.Channel;
            await channel.ConnectAsync();
            var vnext = context.Client.GetVoiceNext();
            var connection = vnext.GetConnection(context.Guild);

            ulong serverId = connection.TargetChannel.GuildId ?? 0;

            if (Bot.Filas[serverId].Count() < 1 || !connection.IsPlaying)
            {

                while (Bot.Filas[context.Guild.Id].Count() > 0)
                {
                    await YtTools.Download(Bot.Filas[context.Guild.Id][0]);

                    string path = getPath(context);

                    await PlayAudioAsync(context, connection, path, Bot.Filas[context.Guild.Id][0]);

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
            string path = getPath(context);

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
        public async Task Pula(CommandContext context)
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
                string path = getPath(context);

                try
                {
                    connection.Disconnect();
                }
                catch (Exception ex) { await context.Channel.SendMessageAsync(ex.Message); }

                File.Delete(path);
                Bot.Filas[context.Guild.Id].RemoveAt(0);
            }

            await channel.ConnectAsync();
            vnext = context.Client.GetVoiceNext();
            connection = vnext.GetConnection(context.Guild);

            while ((Bot.Filas[context.Guild.Id].Count() > 0))
            {

                await YtTools.Download(Bot.Filas[context.Guild.Id][0]);

                string path = getPath(context);
                try
                {
                    await PlayAudioAsync(context, connection, path, Bot.Filas[context.Guild.Id][0]);
                }
                catch (Exception ex) { await context.Channel.SendMessageAsync(ex.Message); }

                Bot.Filas[context.Guild.Id].RemoveAt(0);
                File.Delete(path);
                _cts.Cancel();
            }
            return;
        }

        [Command("d")]
        private async Task download(CommandContext context, params string[] args)
        {
            string msg = "";

            foreach (string arg in args)
            {
                msg += arg + " ";
            }
            
            await YtTools.Download(msg);
        }
        private async Task PlayAudioAsync(CommandContext ctx, VoiceNextConnection connection, string filePath, string msg)
        {            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"O arquivo {filePath} não existe");
                return;
            }

            if (ConfigHandler.Linux)
            {
                try
                {
                    using (var ffmpeg = CreateProcess(filePath))
                    using (var output = ffmpeg.StandardOutput.BaseStream)
                    using (var transmit = connection.GetTransmitSink())
                    {
                        await output.CopyToAsync(transmit, null, _cts.Token);
                    }
                }
                catch (Exception ex)
                {
                    await ctx.RespondAsync($"Erro ao tocar o arquivo: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    using (var mediaReader = new MediaFoundationReader(filePath))
                    {
                        using (var resampler = new MediaFoundationResampler(mediaReader, _waveFormat))
                        {
                            resampler.ResamplerQuality = 60;

                            var buffer = new byte[1024];
                            int bytesRead;

                            var transmit = connection.GetTransmitSink();
                            await ctx.Channel.SendMessageAsync($"Tocando agora: {msg}");

                            while ((bytesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                await transmit.WriteAsync(buffer, 0, bytesRead, _cts.Token);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await ctx.RespondAsync($"Ocorreu um erro: {ex.Message}");
                }
            }
            return;
        }
        private Process CreateProcess(string filePath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{filePath}\" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            return Process.Start(psi);
        }
        private string? getPath(CommandContext context)
        {
            if(Bot.Filas[context.Guild.Id].Count() == 0)
            {
                return null;
            }
            
            string musicPath;

            if (ConfigHandler.Linux)
            {
                musicPath = "/musics/";
            }
            else { musicPath = @"\musics\"; }

            string path = Directory.GetCurrentDirectory() + $@"{musicPath}{Bot.Filas[context.Guild.Id].FirstOrDefault().Replace(' ', '_')}.mp3";

            return path;
        }
    }
}


