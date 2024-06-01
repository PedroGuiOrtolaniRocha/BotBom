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
        static void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Aplicativo está sendo finalizado.");
        }

        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Ctrl+C pressionado. Finalizando aplicativo...");
            Bot.Shutdown();
            Environment.Exit(0); 
        }
    }
}
