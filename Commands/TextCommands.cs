using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NovoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicalBot.Commands
{
    public class TextCommands : BaseCommandModule
    {
        private static string _helpMessage = 
            $"""
            Obrigado por usar o nosso bot,os comandos podem ser acionados por qualquer um dos nomes separados por / independente de letras maiusculas ou minuisculas
            
            **{ConfigHandler.Prefix}p/play/toca <nome ou URL da musica ou playlist do youtube>** 
            Toca a musica selecionada ou adiciona ao fim da fila se estiver tocando.

            **{ConfigHandler.Prefix}add/put/adiciona <nome ou URL da musica> opcional<posição na fila>** 
            Adiciona a musica na posição selecionada, o padráo é o fim da fila, passando as posteriores para proxima posição.

            **{ConfigHandler.Prefix}pula/next opcional<numero de musicas para pular>** 
            Pula a quantidade selecionadas de músicas, o padrão é apenas a que esta tocando.

            **{ConfigHandler.Prefix}remove/tira/retira/rm <posição da musica na fila>**
            Retira a musica da fila na posição selecionada

            **{ConfigHandler.Prefix}f/fila/queue/lista/list**
            Exibe todas as musicas da fila e suas respectivas posições

            **{ConfigHandler.Prefix}sai/leave/quit** 
            Sai do canal de voz e reinicia a fila

            **Se quiser apoiar os desenvolvedores do projeto enviar qualquer valor para**
            db705d0c-4898-49ed-b411-36d6e81c2f36
            """;

        private string _queueMessage;

        private DiscordEmbedBuilder _helpEmbed = new()
        {
            Color = DiscordColor.Red,
            Title = "Ajuda e comandos",
            Description = _helpMessage
        };

        [Command("Ajuda")]
        [Aliases("Help", "h")]
        public async Task Help(CommandContext context)
        {
            await context.Channel.SendMessageAsync(embed: _helpEmbed);
        }
        
        [Command("fila")]
        [Aliases("queue", "list", "lista", "f")]
        public async Task Fila(CommandContext context)
        {
            string queue = "";
            foreach (string[] music in Bot.Filas[context.Guild.Id])
            {
                queue += $"**{Bot.Filas[context.Guild.Id].IndexOf(music) + 1}** - {music[1]}\n";
            }
            
            DiscordEmbedBuilder queueEmbed = new()
            {
                Color = DiscordColor.Red,
                Title = "Fila",
                Description = queue
            };


            await context.Channel.SendMessageAsync(queueEmbed);
            return;
        }
    }
}
