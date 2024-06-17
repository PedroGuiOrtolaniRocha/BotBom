using MusicalBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using NovoBot;
using MusicalBot.YTtools;
using AngleSharp.Dom;

namespace MusicalBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);
            await Bot.Run();
        }
        static async void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Ctrl+C pressionado. Finalizando aplicativo...");
            await Bot.Shutdown();
            Environment.Exit(0); 
        }
    }
}
