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
        public Game Game { get; private set; }
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
            Network = new NetworkManager(client, isHost: true, myTurn: true);
            UI = new UIManager(this, (message) => MessageBox.Show(message));

            // SETUP GAME
            Game = new Game(hostPlayer, peerPlayer, Network, UI);
            Game.Prepare();
            Game.Start();

            // Start listening for incoming messages
            Task.Run(() => ListenForMessages());
        } // Host
        internal FRM_Game(TcpClient client, Player hostPlayer, Player peerPlayer)
        {
            // Setup Form
            InitializeComponent();
            BackColor = _gameBackgroundColor;
            this.Text = "BrawlTCG-alpha Peer";

            // Setup Network
            Network = new NetworkManager(client, isHost: false, myTurn: false);
            UI = new UIManager(this, (message) => MessageBox.Show(message));

            // SETUP GAME
            Game = new Game(peerPlayer, hostPlayer, Network, UI);
            Game.Prepare();
            Game.Start();

            // Start listening for incoming messages
            Task.Run(() => ListenForMessages());
        } // Peer


        // Networking Methods
        async Task ListenForMessages()
        {
            while (true)
            {
                string message = await Network.Reader.ReadLineAsync();  // Use async/await here for non-blocking
                if (message != null)
                {
                    string[] parts = message.Split(':');

                    if (parts[0] == "SWITCH_TURN")
                    {
                        // Use Invoke to safely update the UI from the background thread
                        this.Invoke((Action)(() =>
                        {
                            SwitchTurn();
                        }));
                    }
                    else if (parts[0] == "PLAY_CARD") // PLAY_CARD:HAND_INDEX:<hand-index>:TARGET_ZONE:<target-zone>
                    {
                        if (parts[4] == ZoneTypes.EssenceField.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = Game.Opponent.Hand[handIndex];
                            CardControl oldCC = UI.GetCardControl(Game.Opponent, ZoneTypes.Hand, card);
                            ZoneControl zone = UI.GetMyZone(ZoneTypes.EssenceField, Game.Opponent);
                            this.Invoke((Action)(() =>
                            {
                                UI.PlayEssenceCard(Game.Opponent, card, oldCC, zone);
                            }));
                        }
                        else if (parts[4] == ZoneTypes.Stage.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = Game.Opponent.Hand[handIndex];
                            this.Invoke((Action)(() =>
                            {
                                UI.PlayStageCard(Game.Opponent, (StageCard)card);
                            }));
                        }
                        else if (parts[4] == ZoneTypes.PlayingField.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            LegendCard legendCard = (LegendCard)Game.Opponent.Hand[handIndex];
                            CardControl oldCC = UI.GetCardControl(Game.Opponent, ZoneTypes.Hand, legendCard);
                            ZoneControl zone = UI.GetMyZone(ZoneTypes.PlayingField, Game.Opponent);
                            this.Invoke((Action)(() =>
                            {
                                UI.PlayLegendCard(Game.Opponent, legendCard, oldCC, zone);
                            }));
                        }
                        else if (parts[3] == "TARGET_LEGEND")
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = Game.Opponent.Hand[handIndex];

                            // play wep card
                            if (card is WeaponCard weaponCard)
                            {
                                int indexCC = Convert.ToInt32(parts[4]);
                                ZoneControl playZone = UI.GetMyZone(ZoneTypes.PlayingField, Game.Opponent);
                                CardControl legendCC = playZone.CardsControls[indexCC];
                                CardControl oldCC = UI.GetCardControl(Game.Opponent, ZoneTypes.Hand, weaponCard);
                                this.Invoke((Action)(() =>
                                {
                                    UI.PlayWeaponCard(Game.Opponent, (LegendCard)legendCC.Card, weaponCard, oldCC);
                                }));
                            }
                            // play battle card
                            else if (card is BattleCard battleCard)
                            {
                                int indexCC = Convert.ToInt32(parts[4]); // card control index
                                bool friendlyFire = bool.Parse(parts[6]);
                                Player targetPlayer;
                                ZoneControl targetZone;
                                if (friendlyFire)
                                {
                                    targetPlayer = Game.Opponent;
                                }
                                else
                                {
                                    targetPlayer = Game.Me;
                                }
                                targetZone = UI.GetMyZone(ZoneTypes.PlayingField, targetPlayer);
                                CardControl oldCC = UI.GetCardControl(Game.Opponent, ZoneTypes.Hand, card);
                                CardControl targetCC = targetZone.CardsControls[indexCC];

                                this.Invoke((Action)(() =>
                                {
                                    UI.PlayBattleCard(Game.Opponent, battleCard, oldCC, targetCC);
                                }));
                            }
                        }
                    }
                    else if (parts[0] == "ATTACK_PLAYER")
                    {
                        // Find legend
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = Game.Opponent.PlayingField[fieldIndex];
                        CardControl legendCC = UI.GetCardControl(Game.Opponent, ZoneTypes.PlayingField, card);
                        LegendCard legend = (LegendCard)legendCC.Card;

                        // Find attack
                        string correctAttackName = parts[4];
                        Attack chosenAttack = null;
                        foreach (Attack attack in legend.GetAttacks())
                        {
                            if (attack.Name == correctAttackName)
                            {
                                chosenAttack = attack;
                            }
                        }

                        // perform the attack
                        this.Invoke((Action)(() =>
                        {
                            legendCC.AttackThePlayer(legendCC, Game.Me, chosenAttack);
                        }));
                    }
                    else if (parts[0] == "STATUS_ATTACK")
                    {
                        // Find legend
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = Game.Opponent.PlayingField[fieldIndex];
                        CardControl legendCC = UI.GetCardControl(Game.Opponent, ZoneTypes.PlayingField, card);
                        LegendCard legend = (LegendCard)legendCC.Card;

                        // Find attack
                        string correctAttackName = parts[4];
                        Attack chosenAttack = null;
                        foreach (Attack attack in legend.GetAttacks())
                        {
                            if (attack.Name == correctAttackName)
                            {
                                chosenAttack = attack;
                            }
                        }

                        // perform the attack
                        this.Invoke((Action)(() =>
                        {
                            chosenAttack.Effect.Invoke(legend, null, chosenAttack, Game.ActivePlayer, Game);
                        }));
                    }
                    else if (parts[0] == "ATTACK_LEGEND") // NETWORK_SendMessage($"ATTACK_LEGEND:LEGEND_INDEX:{fieldIndex}:ATTACK:{attack.Name}");
                    {
                        // Find attacker
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = Game.Opponent.PlayingField[fieldIndex];
                        CardControl legendCC = UI.GetCardControl(Game.Opponent, ZoneTypes.PlayingField, card);
                        LegendCard legend = (LegendCard)legendCC.Card;

                        // Find attack
                        string correctAttackName = parts[4];
                        Attack chosenAttack = null;
                        foreach (Attack attack in legend.GetAttacks())
                        {
                            if (attack.Name == correctAttackName)
                            {
                                chosenAttack = attack;
                            }
                        }

                        // Find Target
                        CardControl targetCC;
                        if (chosenAttack.FriendlyFire)
                        {
                            int targetFieldIndex = Convert.ToInt32(parts[6]);
                            Card targetCard = Game.Opponent.PlayingField[targetFieldIndex]; // crash
                            targetCC = UI.GetCardControl(Game.Opponent, ZoneTypes.PlayingField, targetCard);
                        }
                        else
                        {
                            int targetFieldIndex = Convert.ToInt32(parts[6]);
                            Card targetCard = Game.Me.PlayingField[targetFieldIndex]; // crash
                            targetCC = UI.GetCardControl(Game.Me, ZoneTypes.PlayingField, targetCard);
                        }


                        // ATTACK
                        Game.StartAttack(chosenAttack);
                        this.Invoke((Action)(() =>
                        {
                            legendCC.AttackLegendCard(legendCC, targetCC);
                        }));
                    }
                    else if (parts[0] == "RANDOM_CARD_ID")
                    {
                        Game.RandomCardID = Convert.ToInt32(parts[1]);
                    }
                }
            }
        }
        public void SwitchTurn()
        {
            Game.SwitchTurn();
            Game.StartTurn();

            // Update UI (turn indication)
            ZoneControl zone = UI.GetMyZone(ZoneTypes.PlayerInfo, Game.ActivePlayer);
            zone.BackColor = Color.Green;

            ZoneControl zone2 = UI.GetMyZone(ZoneTypes.PlayerInfo, Game.InactivePlayer);
            zone2.BackColor = SystemColors.ControlDarkDark;
        }


        // Events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space && Game.Me == Game.ActivePlayer)
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