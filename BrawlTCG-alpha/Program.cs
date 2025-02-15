using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Visuals;

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
            Application.Run(new FRM_Game(new Player("Odin", CardCatalogue.TestDeck), new Player("Valhalla", CardCatalogue.GsDeck))); // build decks first
            //Application.Run(new FRM_SetupGame()); // build decks first
        }
    }
}