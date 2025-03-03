using BrawlTCG_alpha.Logic.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Logic
{
    internal class PlayerManager
    {
        // Properties
        public Player ActivePlayer { get; private set; }
        public Player InactivePlayer { get; private set; }
        public Player Me { get; private set; }
        public Player Opponent { get; private set; }
        public bool MyTurn { get; set; } = false;

        // Fields
        public event Action<string> UI_PopUpNotification;

        // Methods
        public PlayerManager(Player p1, Player p2, Action<string> ui_popUpNotification)
        {
            // Determine Me & Opponent
            if (p1.IsMe)
            {
                Me = p1;
                Opponent = p2;
            }
            else
            {
                Me = p2;
                Opponent = p1;
            }

            // Host always starts
            if (p1.IsHost)
            {
                ActivePlayer = p1;
                InactivePlayer = p2;
                MyTurn = true;
            }
            else
            {
                ActivePlayer = p2;
                InactivePlayer = p1;
                MyTurn = false;
            }

            // Setup popup
            UI_PopUpNotification += ui_popUpNotification;
        }
        public Player GetOtherPlayer(Player p)
        {
            if (Me == p)
            {
                return Opponent;
            }
            else if (Opponent == p)
            {
                return Me;
            }
            throw new Exception();
        }
        public List<Player> GetPlayers()
        {
            return [ActivePlayer, InactivePlayer];
        }
        public void RandomizeStartingPlayer()
        {
            // Randomize which player starts (0 for top player, 1 for bottom player)
            Random random = new Random();
            int startingPlayer = random.Next(2); // Generates either 0 or 1

            if (startingPlayer == 0)
            {
                // Top player starts
                InactivePlayer = Me;
                ActivePlayer = Opponent;
                MyTurn = false;
                UI_PopUpNotification("Top Player Starts");
            }
            else
            {
                // Bottom player starts
                InactivePlayer = Opponent;
                ActivePlayer = Me;
                MyTurn = true;
                UI_PopUpNotification("Bottom Player Starts");
            }
        }
        public void SwitchActivePlayer()
        {
            MyTurn = !MyTurn;
            if (ActivePlayer == Me)
            {
                ActivePlayer = Opponent;
                InactivePlayer = Me;
            }
            else
            {
                ActivePlayer = Me;
                InactivePlayer = Opponent;
            }
        }
    }
}
