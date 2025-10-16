using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrawlTCG_alpha.Visuals
{
    public partial class FRM_ServerLobby : Form
    {
        private readonly ClientWebSocket _socket;

        // UI controls
        private GroupBox groupBoxLobby;
        private TextBox txtUsername;
        private TextBox txtDeckName;
        private Button BTN_Join_Lobby;
        private Label lblUsername;
        private Label lblDeck;
        private ListBox LB_Players;

        public FRM_ServerLobby(ClientWebSocket socket)
        {
            _socket = socket;
            InitializeComponent();
            InitializeLobbyControls();
        }

        private void InitializeLobbyControls()
        {

            // --- Lobby ListBox ---
            LB_Players = new ListBox();
            LB_Players.Top = 300;
            LB_Players.Left = 20;
            LB_Players.Width = 300;
            LB_Players.Height = 150;
            LB_Players.Name = "listBoxPlayers";
            this.Controls.Add(LB_Players);
        }

        private async Task ReceiveLobbyUpdatesAsync()
        {
            var buffer = new byte[1024];

            try
            {
                while (_socket.State == WebSocketState.Open)
                {
                    var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", CancellationToken.None);
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    // Parse JSON
                    using var jsonDoc = JsonDocument.Parse(message);
                    string msgType = jsonDoc.RootElement.GetProperty("type").GetString();

                    if (msgType == "lobby_update")
                    {
                        var players = jsonDoc.RootElement.GetProperty("data").GetProperty("players");

                        // Update UI safely
                        this.Invoke((Action)(() =>
                        {
                            LB_Players.Items.Clear();
                            foreach (var player in players.EnumerateArray())
                            {
                                string playerName = player.GetProperty("player_name").GetString();
                                string deckName = player.GetProperty("deck_name").GetString();
                                LB_Players.Items.Add($"{playerName} ({deckName})");
                            }
                        }));
                    }

                    if (msgType == "start_game")
                    {
                        this.Invoke((Action)(() =>
                        {
                            MessageBox.Show("The game is starting!");
                            // TODO: Hide lobby, open game form
                        }));
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Receive error: " + ex.Message);
            }
        }

        async private void BTN_Join_Lobby_2_Click(object sender, EventArgs e)
        {
            string username = TB_Username.Text.Trim();
            string deckName = TB_Deck.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(deckName))
            {
                MessageBox.Show("Please enter both username and deck name!");
                return;
            }

            // Send join lobby message to server
            var joinMessage = new
            {
                type = "join_lobby",
                data = new { player_name = username, deck_name = deckName }
            };

            string json = JsonSerializer.Serialize(joinMessage);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            try
            {
                await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                MessageBox.Show("Join request sent!");

                // Start receiving lobby updates in the background
                _ = ReceiveLobbyUpdatesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending join request: " + ex.Message);
            }
        }
    }
}
