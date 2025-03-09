using BrawlTCG_alpha.Logic;
using Mono.Nat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrawlTCG_alpha.Visuals
{
    public partial class FRM_Menu : Form
    {
        int _port = 5001;
        private INatDevice _natDevice; // Store the discovered NAT device
        // Host variables
        TcpListener _host;
        // Client Variables
        TcpClient _client;
        NetworkStream _stream;
        StreamReader _streamReader;
        StreamWriter _streamWriter;
        public FRM_Menu()
        {
            InitializeComponent();

            // Start discovering UPnP devices
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.StartDiscovery();
        }

        // This event is triggered when a new device (router) is discovered
        private void DeviceFound(object sender, DeviceEventArgs args)
        {
            // Store the discovered device
            _natDevice = args.Device;
            Invoke((Action)(() =>
            {
                SetStatus("Router discovered!");
            }));

            // Example: Create a port map for TCP port 5001
            CreatePortMapAsync(_port);
        }

        // Method to create a port map
        private async Task CreatePortMapAsync(int port)
        {
            if (_natDevice == null)
            {
                MessageBox.Show("No UPnP device found. Port forwarding cannot be done.");
                return;
            }

            try
            {
                // Create a port mapping for the given port
                Mapping mapping = new Mapping(Protocol.Tcp, port, port);
                await Task.Run(() => _natDevice.CreatePortMap(mapping)); // Run port mapping asynchronously

                Invoke((Action)(() =>
                {
                    SetStatus($"Port {port} forwarded.");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error forwarding port: {ex.Message}");
            }
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

            // Get player deck
            List<Card> playerDeck = Deck.LoadDeckFromFile(TB_Deck.Text + ".txt");
            playerDeck = ShuffleDeck(playerDeck);
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

                await JoinGame(hostIp, playerData, playerDeck);
            }
        }

        // Helper method to update the UI status
        private void SetStatus(string message)
        {
            LBL_Status.Text = message;
            this.Update();
        }

        // Example method to start hosting a game
        private async Task HostGame(string playerData, List<Card> playerDeck)
        {
            try
            {
                // Create a TcpListener to wait for incoming connections
                _host = new TcpListener(System.Net.IPAddress.Any, _port);
                _host.Start();
                SetStatus("Waiting for second player...");

                // Wait for a peer to connect first
                _client = await _host.AcceptTcpClientAsync();
                SetStatus("Someone is joining...");

                // Setup reader and writer for communication after the connection
                _stream = _client.GetStream();
                _streamReader = new StreamReader(_stream);
                _streamWriter = new StreamWriter(_stream) { AutoFlush = true };
                SetStatus("Someone joined!");

                // After the connection, use Mono.Nat to get the external IP and forward the port
                if (_natDevice != null)
                {
                    try
                    {
                        // Create a port mapping for the host's port (5000 in this case)
                        Mapping mapping = new Mapping(Protocol.Tcp, _port, _port);
                        await Task.Run(() => _natDevice.CreatePortMap(mapping)); // Run port mapping asynchronously

                        // Get the external IP address
                        string externalIp = _natDevice.GetExternalIP().ToString();

                        // Send the external IP address to the peer (via streamWriter)
                        _streamWriter.WriteLine($"HOST_IP:{externalIp}:PLAYER_DATA:{playerData}");
                        SetStatus($"Port forwarded! Your external IP is {externalIp}. Now the peer can connect.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error forwarding port: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("No UPnP device found. Port forwarding cannot be done.");
                    return;
                }

                // Send player data to peer (now that the connection is established)
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

                //_host.Stop(); // Optionally stop the host, depending on your use case.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error hosting game: {ex.Message}");
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

        private async Task JoinGame(string hostIp, string playerData, List<Card> playerDeck)
        {
            try
            {
                // Connect to the host using their external IP
                SetStatus("Joining host...");
                _client = new TcpClient(hostIp, _port);  // Connect to the host's external IP
                _stream = _client.GetStream();
                _streamReader = new StreamReader(_stream);
                _streamWriter = new StreamWriter(_stream) { AutoFlush = true };
                SetStatus("Connected to host!");

                // Send player data to the host
                _streamWriter.WriteLine(playerData);

                // Wait for host data, which contains the player data for the host
                string hostData = await _streamReader.ReadLineAsync();

                // Parse the host's data, which should include the external IP and player data
                string[] hostDataParts = hostData.Split(':');

                if (hostDataParts.Length != 4 || hostDataParts[0] != "HOST_IP")
                {
                    MessageBox.Show("Invalid data received from host.");
                    throw new Exception("Invalid host data.");
                }

                // Extract the external IP and player data
                string externalIp = hostDataParts[1];
                string hostPlayerData = hostDataParts[2];

                // Create player objects based on the received data
                Player hostPlayer = ProcesPeerData(hostPlayerData, isHost: true, isMe: false); // Host
                Player peerPlayer = new Player(playerData.Split(':')[1], playerDeck, isHost: false, isMe: true); // Player joining

                // Start the game with both players
                this.Invoke((Action)(() =>
                {
                    new FRM_Game(this, _client, hostPlayer, peerPlayer).Show();
                    this.Hide();
                }));

                //_client.Close(); // Optionally close the connection after the game starts, depending on your design
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error joining game: {ex.Message}");
            }
        }



    }
}
