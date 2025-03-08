using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BrawlTCG_alpha.Logic
{
    public class NetworkManager
    {
        // Properties
        private TcpClient _client;
        private bool _isHost, _myTurn;
        public StreamWriter Writer { get; private set; }
        public StreamReader Reader { get; private set; }
        FRM_Game _frm;

        // Methods
        public NetworkManager(FRM_Game frm, TcpClient client, bool isHost, bool myTurn)
        {
            // Parameters
            _frm = frm;
            _client = client;
            _isHost = isHost;
            _myTurn = myTurn;

            // Setup Network
            NetworkStream stream = client.GetStream();
            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public void SendMessageToPeer(string message)
        {
            try
            {
                if (Writer != null)
                {
                    Writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}");
            }
        }

        public async Task ListenForMessages()
        {
            Game game = _frm.game;
            UIManager ui = _frm.UI;
            while (true)
            {
                string message = await Reader.ReadLineAsync();  // Use async/await here for non-blocking
                if (message != null)
                {
                    string[] parts = message.Split(':');

                    if (parts[0] == "SWITCH_TURN")
                    {
                        // Use Invoke to safely update the UI from the background thread
                        _frm.Invoke((Action)(() =>
                        {
                            _frm.SwitchTurn();
                        }));
                    }
                    else if (parts[0] == "PLAY_CARD") // PLAY_CARD:HAND_INDEX:<hand-index>:TARGET_ZONE:<target-zone>
                    {
                        if (parts[4] == ZoneTypes.EssenceField.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = game.Opponent.Hand[handIndex];
                            CardControl oldCC = ui.GetCardControl(game.Opponent, ZoneTypes.Hand, card);
                            ZoneControl zone = ui.GetMyZone(ZoneTypes.EssenceField, game.Opponent);
                            _frm.Invoke((Action)(() =>
                            {
                                ui.PlayEssenceCard(game.Opponent, card, oldCC, zone);
                            }));
                        }
                        else if (parts[4] == ZoneTypes.Stage.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = game.Opponent.Hand[handIndex];
                            _frm.Invoke((Action)(() =>
                            {
                                ui.PlayStageCard(game.Opponent, (StageCard)card);
                            }));
                        }
                        else if (parts[4] == ZoneTypes.PlayingField.ToString())
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            LegendCard legendCard = (LegendCard)game.Opponent.Hand[handIndex];
                            _frm.Invoke((Action)(() =>
                            {
                                ui.PlayLegendCard(game.Opponent, legendCard);
                            }));
                        }
                        else if (parts[3] == "TARGET_LEGEND")
                        {
                            int handIndex = Convert.ToInt32(parts[2]);
                            Card card = game.Opponent.Hand[handIndex];

                            // play wep card
                            if (card is WeaponCard weaponCard)
                            {
                                int indexCC = Convert.ToInt32(parts[4]);
                                ZoneControl playZone = ui.GetMyZone(ZoneTypes.PlayingField, game.Opponent);
                                CardControl legendCC = playZone.CardsControls[indexCC];
                                CardControl oldCC = ui.GetCardControl(game.Opponent, ZoneTypes.Hand, weaponCard);
                                _frm.Invoke((Action)(() =>
                                {
                                    ui.PlayWeaponCard(game.Opponent, (LegendCard)legendCC.Card, weaponCard, oldCC);
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
                                    targetPlayer = game.Opponent;
                                }
                                else
                                {
                                    targetPlayer = game.Me;
                                }
                                targetZone = ui.GetMyZone(ZoneTypes.PlayingField, targetPlayer);
                                CardControl oldCC = ui.GetCardControl(game.Opponent, ZoneTypes.Hand, card);
                                CardControl targetCC = targetZone.CardsControls[indexCC];

                                _frm.Invoke((Action)(() =>
                                {
                                    ui.PlayBattleCard(game.Opponent, battleCard, oldCC, targetCC);
                                }));
                            }
                        }
                    }
                    else if (parts[0] == "ATTACK_PLAYER")
                    {
                        // Find legend
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = game.Opponent.PlayingField[fieldIndex];
                        CardControl legendCC = ui.GetCardControl(game.Opponent, ZoneTypes.PlayingField, card);
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
                        _frm.Invoke((Action)(() =>
                        {
                            legendCC.AttackThePlayer(legendCC, game.Me, chosenAttack);
                        }));
                    }
                    else if (parts[0] == "STATUS_ATTACK")
                    {
                        // Find legend
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = game.Opponent.PlayingField[fieldIndex];
                        CardControl legendCC = ui.GetCardControl(game.Opponent, ZoneTypes.PlayingField, card);
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
                        _frm.Invoke((Action)(() =>
                        {
                            chosenAttack.Effect.Invoke(legend, null, chosenAttack, game.ActivePlayer, game);
                        }));
                    }
                    else if (parts[0] == "ATTACK_LEGEND") // NETWORK_SendMessage($"ATTACK_LEGEND:LEGEND_INDEX:{fieldIndex}:ATTACK:{attack.Name}");
                    {
                        // Find attacker
                        int fieldIndex = Convert.ToInt32(parts[2]);
                        Card card = game.Opponent.PlayingField[fieldIndex];
                        CardControl legendCC = ui.GetCardControl(game.Opponent, ZoneTypes.PlayingField, card);
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
                            Card targetCard = game.Opponent.PlayingField[targetFieldIndex]; // crash
                            targetCC = ui.GetCardControl(game.Opponent, ZoneTypes.PlayingField, targetCard);
                        }
                        else
                        {
                            int targetFieldIndex = Convert.ToInt32(parts[6]);
                            Card targetCard = game.Me.PlayingField[targetFieldIndex]; // crash
                            targetCC = ui.GetCardControl(game.Me, ZoneTypes.PlayingField, targetCard);
                        }


                        // ATTACK
                        game.StartAttack(chosenAttack);
                        _frm.Invoke((Action)(() =>
                        {
                            legendCC.AttackLegendCard(legendCC, targetCC);
                        }));
                    }
                    else if (parts[0] == "RANDOM_CARD_ID")
                    {
                        game.RandomCardID = Convert.ToInt32(parts[1]);
                    }
                }
            }
        }

    }
}
