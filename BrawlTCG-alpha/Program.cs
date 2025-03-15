using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Visuals;
using System.Net.Sockets;

namespace BrawlTCG_alpha
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FRM_Menu()); // build decks first
            //Application.Run(new FRM_Game(new FRM_Menu(), new TcpListener(System.Net.IPAddress.Any, 5000), new TcpClient("127.0.0.1", 5000), new Player("F", new List<Card>(), true, true), new Player("F", new List<Card>(), true, true))); // build decks first
            //Application.Run(new FRM_Game(new FRM_Menu(), new TcpClient("127.0.0.1", 5000), new Player("F", new List<Card>(), true, true), new Player("F", new List<Card>(), true, true))); // build decks first
        }
    }
}