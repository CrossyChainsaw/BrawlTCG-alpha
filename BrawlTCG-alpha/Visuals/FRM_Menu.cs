using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrawlTCG_alpha.Visuals
{
    public partial class FRM_Menu : Form
    {
        // Deck
        const int MINIMUM_CARDS_IN_DECK = 40;
        
        // P2P
        // Host variables
        TcpListener _host;
        // Client Variables
        TcpClient _client;
        NetworkStream _stream;
        StreamReader _streamReader;
        StreamWriter _streamWriter;

        // Client-Server
        private ClientWebSocket socket;

        // Methods
        public FRM_Menu()
        {
            InitializeComponent();
        }

        private void BTN_EditDeck_OnClick(object sender, EventArgs e)
        {
            Form frm = new FRM_DeckBuilder();
            frm.Show();
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
            // Get player name
            string playerName = TB_Name.Text;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Please enter your name.");
                return;
            }

            // Get Player Deck
            List<Card> playerDeck = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = "Select a Deck File";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                playerDeck = Deck.LoadDeckFromFile(selectedFile);
            }
            else
            {
                MessageBox.Show("No deck selected.");
                return;
            }

            playerDeck = ShuffleDeck(playerDeck);
            // validate deck (put this in method)
            if (playerDeck == null || playerDeck.Count == 0)
            {
                MessageBox.Show("Your deck is empty! Please select or build a deck before playing.");
                return;
            }
            if (playerDeck == null || playerDeck.Count < MINIMUM_CARDS_IN_DECK)
            {
                MessageBox.Show($"Your deck does not contain at least {MINIMUM_CARDS_IN_DECK} cards. Your deck: {playerDeck.Count}");
                return;
            }

            // Convert deck to a string of card IDs
            string deckString = string.Join(",", playerDeck.Select(card => card.ID));

            // Prepare player data string
            string playerData = $"PLAYER_NAME:{playerName}:PLAYER_DECK:{deckString}";

            // Prompt to choose to host or join a game
            DialogResult dialogResult = MessageBox.Show("Do you want to host a game?", "Host or Join", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Host: Set up the listener and wait for connection from peer
                await HostGame(playerData, playerDeck);
            }
            else
            {
                // Join: Ask for the host's IP and connect to it
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
                // Create a TcpListener to wait for incoming connections
                _host = new TcpListener(System.Net.IPAddress.Any, 5000);
                _host.Start();
                SetStatus("Waiting for second player...");

                // Wait for a peer to connect
                _client = await _host.AcceptTcpClientAsync();
                SetStatus("Someone is joining...");

                // Setup reader and writer for communication
                _stream = _client.GetStream();
                _streamReader = new StreamReader(_stream);
                _streamWriter = new StreamWriter(_stream) { AutoFlush = true };
                SetStatus("Someone joined!");


                // Send player data to peer
                _streamWriter.WriteLine(playerData);


                // Wait for player data from the peer
                string peerData = await _streamReader.ReadLineAsync();

                // Create player objects
                Player hostPlayer = new Player(playerData.Split(':')[1], playerDeck, isHost: true, isMe: true); // Host
                Player peerPlayer = ProcesPeerData(peerData, isHost: false, isMe: false);

                // Start the game with two players
                this.Invoke((Action)(() =>
                {
                    new FRM_Game(this, _host, _client, peerPlayer, hostPlayer).Show();
                    this.Hide();
                }));

                // Close the listener
                //_host.Stop();
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
                // Connect to the host
                SetStatus("Joining host...");
                _client = new TcpClient(hostIp, 5000);
                _stream = _client.GetStream();
                _streamReader = new StreamReader(_stream);
                _streamWriter = new StreamWriter(_stream) { AutoFlush = true };
                SetStatus("Connected to host!");


                // Send player data to the host
                _streamWriter.WriteLine(playerData);

                // Wait for player data from the host
                string hostData = await _streamReader.ReadLineAsync();

                // Create player objects
                Player hostPlayer = ProcesPeerData(hostData, isHost: true, isMe: false);
                Player peerPlayer = new Player(playerData.Split(':')[1], playerDeck, isHost: false, isMe: true); // Player joining

                // Start the game with two players
                this.Invoke((Action)(() =>
                {
                    new FRM_Game(this, _client, hostPlayer, peerPlayer).Show();
                    this.Hide();
                }));

                //_client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error joining game: {ex.Message}");
            }
        }

        Player ProcesPeerData(string peerData, bool isHost, bool isMe)
        {
            // check if data is null
            if (string.IsNullOrEmpty(peerData))
            {
                MessageBox.Show("No data received from peer.");
                throw new Exception();
            }

            // Parse the peer data
            string[] peerDataParts = peerData.Split(':');
            if (peerDataParts.Length != 4 || peerDataParts[0] != "PLAYER_NAME")
            {
                MessageBox.Show("Invalid data received from peer.");
                throw new Exception();
            }

            // Parse player name and deck from peer data
            string peerName = peerDataParts[1];
            string peerDeckString = peerDataParts[3];
            List<int> peerCardIds = peerDeckString.Split(',').Select(id => int.Parse(id)).ToList();

            // Get peer's cards by their IDs from CardCatalogue
            List<Card> peerDeck = peerCardIds.Select(id => CardCatalogue.GetCardById(id)).ToList();

            Player player2 = new Player(peerName, peerDeck, isHost, isMe); // Peer
            return player2;
        }

        private void TB_Deck_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private async void BTN_Connect_Click(object sender, EventArgs e)
        {
            socket = new ClientWebSocket();

            try
            {
                // 1️⃣ Attempt connection to Python WebSocket server
                await socket.ConnectAsync(new Uri("ws://localhost:8765"), CancellationToken.None);

                if (socket.State == WebSocketState.Open)
                {
                    MessageBox.Show("✅ Connected to server successfully!");
                    this.Hide();
                    var lobbyForm = new FRM_ServerLobby(socket);
                    lobbyForm.FormClosed += (s, args) => this.Close(); // ensures app exits when lobby closes
                    lobbyForm.Show();
                }
                else
                {
                    MessageBox.Show("⚠️ Failed to connect. Current state: " + socket.State);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Connection error: " + ex.Message);
            }
        }
    }
}
