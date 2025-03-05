#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using Microsoft.VisualBasic.Devices;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Policy;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace BrawlTCG_alpha
{
    public partial class FRM_Game : Form
    {
        // Properties
        public NetworkManager Network { get; private set; }
        public Game game { get; private set; }
        public UIManager UI;
        Color _gameBackgroundColor = Color.MidnightBlue;
        

        // Methods
        internal FRM_Game(TcpListener host, TcpClient client, Player hostPlayer, Player peerPlayer)
        {
            // Setup
            InitializeComponent();
            BackColor = _gameBackgroundColor;
            this.Text = "BrawlTCG-alpha Host";

            // Setup Network
            Network = new NetworkManager(this, client, isHost: true, myTurn: true);
            UI = new UIManager(this, (message) => MessageBox.Show(message));

            // SETUP GAME
            game = new Game(hostPlayer, peerPlayer, Network, UI);
            game.Prepare();
            game.Start();

            // Start listening for incoming messages
            Task.Run(() => Network.ListenForMessages());
        } // Host
        internal FRM_Game(TcpClient client, Player hostPlayer, Player peerPlayer)
        {
            // Setup Form
            InitializeComponent();
            BackColor = _gameBackgroundColor;
            this.Text = "BrawlTCG-alpha Peer";

            // Setup Network
            Network = new NetworkManager(this, client, isHost: false, myTurn: false);
            UI = new UIManager(this, (message) => MessageBox.Show(message));

            // SETUP GAME
            game = new Game(peerPlayer, hostPlayer, Network, UI);
            game.Prepare();
            game.Start();

            // Start listening for incoming messages
            Task.Run(() => Network.ListenForMessages());
        } // Peer


        // Networking Methods
        public void SwitchTurn()
        {
            game.SwitchTurn();
            game.StartTurn();

            // Update UI (turn indication)
            ZoneControl zone = UI.GetMyZone(ZoneTypes.PlayerInfo, game.ActivePlayer);
            zone.BackColor = Color.Green;

            ZoneControl zone2 = UI.GetMyZone(ZoneTypes.PlayerInfo, game.InactivePlayer);
            zone2.BackColor = SystemColors.ControlDarkDark;
        }


        // Events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space && game.Me == game.ActivePlayer)
            {
                // Send END_TURN to the server
                Network.SendMessageToPeer("SWITCH_TURN");
                SwitchTurn();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        } // COMMUNICATION !
    }

}



#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.