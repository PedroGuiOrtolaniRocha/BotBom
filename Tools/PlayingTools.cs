﻿using AngleSharp.Dom;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using NAudio.Wave;
using NovoBot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicalBot.Tools
{
    public class PlayingTools
    {
        public async ValueTask PlayAudioAsync(CommandContext ctx, VoiceNextConnection connection, string filePath, string msg, CancellationToken cts)
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
                        await output.CopyToAsync(transmit, null, cts);
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
                        using (var resampler = new MediaFoundationResampler(mediaReader, new WaveFormat(48000, 16, 2)))
                        {
                            resampler.ResamplerQuality = 60;

                            var buffer = new byte[1024];
                            int bytesRead;

                            var transmit = connection.GetTransmitSink();
                            await ctx.Channel.SendMessageAsync($"Tocando agora: {msg}");

                            while ((bytesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                await transmit.WriteAsync(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await ctx.RespondAsync($"Ocorreu um erro: {ex.Message}");
                }
            }
            Console.WriteLine("Cheguei no fim da função");
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
        public string? getPath(CommandContext context)
        {
            var queue = Bot.Filas[context.Guild.Id];

            if (queue.Count() == 0)
            {
                return null;
            }

            string musicPath;
            string path ="";

            if (ConfigHandler.Linux)
            {
                musicPath = "/musics/";
            }
            else { musicPath = @"\musics\"; }

            if(!Bot.Filas[context.Guild.Id].FirstOrDefault().Contains("www.youtube"))
            {
                path = Directory.GetCurrentDirectory() + $@"{musicPath}{queue.FirstOrDefault()[0].Replace(' ', '_')}.mp3";
            }
            else 
            {
                Console.WriteLine("aq");
                string music = queue[0][0];
                if (music.Contains('&'))
                {
                    music = music.Remove(music.IndexOf('&'));
                }
                music = music.Remove(0, music.IndexOf('=') + 1);
                string musicFile = music.Trim();
                path = Directory.GetCurrentDirectory() + $@"{musicPath}{musicFile.Replace(' ', '_')}.mp3";
            }
            

            return path;
        }
    }
}
