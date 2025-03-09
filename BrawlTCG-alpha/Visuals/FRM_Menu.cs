using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Visuals;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrawlTCG_alpha.Visuals
{
    public partial class FRM_Menu : Form
    {
        // Host variables
        TcpListener _host;
        // Client Variables
        TcpClient _client;
        NetworkStream _stream;
        StreamReader _streamReader;
        StreamWriter _streamWriter;

        // UPnP variables
        private NatDevice _upnpDevice;
        private int _port = 5000; // Port for TCP connection

        public FRM_Menu()
        {
            InitializeComponent();
            DiscoverUPnP(); // Discover UPnP support on startup
        }

        private void BTN_EditDeck_OnClick(object sender, EventArgs e)
        {
            Form frm = new FRM_DeckBuilder();
            frm.Show();
        }

        private async void BTN_OnlineMultiplayer_Click(object sender, EventArgs e)
        {
            MessageBox.Show("No");
        }

        List<Card> ShuffleDeck(List<Card> deck)
        {
            Random rng = new Random();
            int n = deck.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]); // Swap
            }
            return deck;
        }

        private async void BTN_P2P_Click(object sender, EventArgs e)
        {
            string playerName = TB_Name.Text;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Please enter your name.");
                return;
            }

            List<Card> playerDeck = Deck.LoadDeckFromFile(TB_Deck.Text + ".txt");
            playerDeck = ShuffleDeck(playerDeck);
            if (playerDeck == null || playerDeck.Count == 0)
            {
                MessageBox.Show("Your deck is empty! Please select or build a deck before playing.");
                return;
            }

            string deckString = string.Join(",", playerDeck.Select(card => card.ID));
            string playerData = $"PLAYER_NAME:{playerName}:PLAYER_DECK:{deckString}";

            DialogResult dialogResult = MessageBox.Show("Do you want to host a game?", "Host or Join", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                await HostGame(playerData, playerDeck);
            }
            else
            {
                string hostIp = Microsoft.VisualBasic.Interaction.InputBox("Enter the host's IP address:", "Host IP", "127.0.0.1");
                if (string.IsNullOrWhiteSpace(hostIp))
                {
                    MessageBox.Show("Invalid IP address.");
                    return;
                }

                await JoinGame(hostIp, playerData, playerDeck);
            }
        }

        void SetStatus(string s)
        {
            LBL_Status.Text = s;
            this.Update();
        }

        private async Task HostGame(string playerData, List<Card> playerDeck)
        {
            try
            {
                if (_upnpDevice != null)
                {
                    await _upnpDevice.CreatePortMapAsync(new Mapping(Protocol.Tcp, _port, _port, "BrawlTCG Host"));
                    SetStatus("UPnP port forwarded!");
                }

                _host = new TcpListener(IPAddress.Any, _port);
                _host.Start();
                SetStatus("Waiting for second player...");

                _client = await _host.AcceptTcpClientAsync();
                SetStatus("Someone joined!");

                _stream = _client.GetStream();
                _streamReader = new StreamReader(_stream);
                _streamWriter = new StreamWriter(_stream) { AutoFlush = true };

                _streamWriter.WriteLine(playerData);
                string peerData = await _streamReader.ReadLineAsync();

                Player hostPlayer = new Player(playerData.Split(':')[1], playerDeck, isHost: true, isMe: true);
                Player peerPlayer = ProcessPeerData(peerData, isHost: false, isMe: false);

                this.Invoke((Action)(() =>
                {
                    new FRM_Game(this, _host, _client, peerPlayer, hostPlayer).Show();
                    this.Hide();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error hosting game: {ex.Message}");
            }
        }

        private async Task JoinGame(string hostIp, string playerData, List<Card> playerDeck)
        {
            try
            {
                SetStatus("Joining host...");
                _client = new TcpClient(hostIp, _port);
                _stream = _client.GetStream();
                _streamReader = new StreamReader(_stream);
                _streamWriter = new StreamWriter(_stream) { AutoFlush = true };
                SetStatus("Connected to host!");

                _streamWriter.WriteLine(playerData);
                string hostData = await _streamReader.ReadLineAsync();

                Player hostPlayer = ProcessPeerData(hostData, isHost: true, isMe: false);
                Player peerPlayer = new Player(playerData.Split(':')[1], playerDeck, isHost: false, isMe: true);

                this.Invoke((Action)(() =>
                {
                    new FRM_Game(this, _client, hostPlayer, peerPlayer).Show();
                    this.Hide();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error joining game: {ex.Message}");
            }
        }

        Player ProcessPeerData(string peerData, bool isHost, bool isMe)
        {
            if (string.IsNullOrEmpty(peerData))
            {
                MessageBox.Show("No data received from peer.");
                throw new Exception();
            }

            string[] peerDataParts = peerData.Split(':');
            if (peerDataParts.Length != 4 || peerDataParts[0] != "PLAYER_NAME")
            {
                MessageBox.Show("Invalid data received from peer.");
                throw new Exception();
            }

            string peerName = peerDataParts[1];
            string peerDeckString = peerDataParts[3];
            List<int> peerCardIds = peerDeckString.Split(',').Select(id => int.Parse(id)).ToList();

            List<Card> peerDeck = peerCardIds.Select(id => CardCatalogue.GetCardById(id)).ToList();

            return new Player(peerName, peerDeck, isHost, isMe);
        }

        private async void DiscoverUPnP()
        {
            try
            {
                var discoverer = new NatDiscoverer();
                var cts = new System.Threading.CancellationTokenSource(5000); // Timeout after 5 seconds
                _upnpDevice = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                SetStatus("UPnP Device Found!");
            }
            catch
            {
                SetStatus("UPnP not available.");
                _upnpDevice = null;
            }
        }

        private async void FRM_Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_upnpDevice != null)
            {
                await _upnpDevice.DeletePortMapAsync(new Mapping(Protocol.Tcp, _port, _port));
                SetStatus("UPnP port removed.");
            }
        }
    }
}
