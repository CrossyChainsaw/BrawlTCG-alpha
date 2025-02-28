using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Visuals;
using Newtonsoft.Json; // For JSON serialization
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private async void BTN_OnlineMultiplayer_Click(object sender, EventArgs e)
        {
            MessageBox.Show("No");
            //ConnectToServer();

            //// Get player name
            //string playerName = TB_Name.Text;
            //if (string.IsNullOrWhiteSpace(playerName))
            //{
            //    MessageBox.Show("Please enter your name.");
            //    return;
            //}

            //// Load deck from file
            //List<Card> playerDeck = Deck.LoadDeckFromFile(TB_Deck.Text + ".txt");
            //if (playerDeck == null || playerDeck.Count == 0)
            //{
            //    MessageBox.Show("Your deck is empty! Please select or build a deck before playing.");
            //    return;
            //}

            //// Convert deck to a string (ensure proper formatting)
            //string deckString = string.Join(",", playerDeck.Select(card => card.ID));

            //// Prepare player data string to send to the server in custom format
            //string playerData = $"PLAYER_NAME:{playerName}:PLAYER_DECK:{deckString}";
            //_streamWriter.WriteLine(playerData);
            //_streamWriter.Flush(); // Ensure data is sent immediately


            //MessageBox.Show("Waiting for another player to connect...");

            //// Wait for the server to send the player data
            //string serverResponse = await _streamReader.ReadLineAsync();
            //if (string.IsNullOrEmpty(serverResponse))
            //{
            //    MessageBox.Show("Connection lost or player data not received.");
            //    return;
            //}

            //// Parse the received server response
            //string[] dataParts = serverResponse.Split(':');
            //if (dataParts.Length != 3 || dataParts[0] != "PLAYERS_DATA")
            //{
            //    MessageBox.Show("Invalid data received from server.");
            //    return;
            //}

            //// Parse player 1 and player 2 data from the response
            //string player1Data = dataParts[1]; // Format: playerName|deckID1,deckID2,...
            //string player2Data = dataParts[2]; // Format: playerName|deckID1,deckID2,...

            //// Extract player names and deck IDs
            //string[] player1Parts = player1Data.Split('|');
            //string[] player2Parts = player2Data.Split('|');

            //if (player1Parts.Length != 2 || player2Parts.Length != 2)
            //{
            //    MessageBox.Show("Invalid player data format received from server.");
            //    return;
            //}

            //string player1Name = player1Parts[0];
            //string player2Name = player2Parts[0];

            //// Parse the deck IDs into lists of integers
            //List<int> player1CardIds = player1Parts[1].Split(',').Select(id => int.Parse(id)).ToList();
            //List<int> player2CardIds = player2Parts[1].Split(',').Select(id => int.Parse(id)).ToList();

            //// Get cards by their IDs from CardCatalogue
            //List<Card> player1Deck = player1CardIds.Select(id => CardCatalogue.GetCardById(id)).ToList();
            //List<Card> player2Deck = player2CardIds.Select(id => CardCatalogue.GetCardById(id)).ToList();

            //// Create player objects
            //Player player1 = new Player(player1Name, player1Deck);
            //Player player2 = new Player(player2Name, player2Deck);

            //MessageBox.Show($"Match found! Your opponent is {player2.Name}");

            //Player me;
            //if (player1.Name == TB_Name.Text)
            //{
            //    me = player1;
            //}
            //else
            //{
            //    me = player2;
            //}

            //// Start the game
            //this.Invoke((Action)(() =>
            //{
            //    new FRM_Game(player1, player2, me).Show();
            //}));

        }

        private void ConnectToServer()
        {
            try
            {
                _client = new TcpClient("127.0.0.1", 5000); // Connect to localhost server
                NetworkStream stream = _client.GetStream();
                _streamReader = new StreamReader(stream);
                _streamWriter = new StreamWriter(stream) { AutoFlush = true };

                MessageBox.Show("Connected to server!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to server: {ex.Message}");
            }
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

            // Get player deck
            List<Card> playerDeck = Deck.LoadDeckFromFile(TB_Deck.Text + ".txt");
            if (playerDeck == null || playerDeck.Count == 0)
            {
                MessageBox.Show("Your deck is empty! Please select or build a deck before playing.");
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

                await JoinGame(hostIp, playerData);
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
                    new FRM_Game(_host, _client, peerPlayer, hostPlayer).Show();
                }));

                // Close the listener
                //_host.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error hosting game: {ex.Message}");
            }
        }

        private async Task JoinGame(string hostIp, string playerData)
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
                Player peerPlayer = new Player(playerData.Split(':')[1], Deck.LoadDeckFromFile(TB_Deck.Text + ".txt"), isHost: false, isMe: true); // Player joining

                // Start the game with two players
                this.Invoke((Action)(() =>
                {
                    new FRM_Game(_client, hostPlayer, peerPlayer).Show();
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
    }
}
