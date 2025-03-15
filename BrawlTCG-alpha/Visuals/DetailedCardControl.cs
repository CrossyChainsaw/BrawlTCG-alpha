using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using BrawlTCG_alpha.Logic.Managers;

namespace BrawlTCG_alpha.Visuals
{
    public class DetailedCardControl : Control
    {
        // Fields
        public Player Owner;
        public List<Player> Players;
        public CardControl OriginalCardControl;
        Game _game;
        List<Button> _attackButtons;
        bool _isRemoved = false;
        PaintCardManager _paintCardManager;
        int _scale = 3;
        public Card Card { get; private set; }
        public List<CardControl> CardsControls { get; internal set; }
        public List<DetailedCardControl> WeaponCardControls { get; internal set; } // the big weapons when showing details


        // Events
        public event Action<Player> UI_UpdatePlayerInformation;
        public event Action<Player> UI_ArrangeCardsInPlayingField;
        public event Action<string> NETWORK_SendMessage;

        public DetailedCardControl(Game game, Card card, Player owner, List<Player> players, CardControl originalCardControl, Action<Player> arrangeCards)
        {
            Card = card;
            Size = new Size(150, 200);
            BackColor = card.CardColor;
            ForeColor = card.TextColor;
            Font = new Font("Arial", 12, FontStyle.Bold);
            this.Click += (sender, e) => OnDetailedCardClicked();
            CardsControls = new List<CardControl>();
            WeaponCardControls = new List<DetailedCardControl>();
            _attackButtons = new List<Button>();
            Owner = owner;
            Players = players;
            OriginalCardControl = originalCardControl;
            UI_ArrangeCardsInPlayingField = arrangeCards;
            _game = game;
            _paintCardManager = new PaintCardManager();
        }


        // Paint Cards
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (Card is LegendCard legendCard)
            {
                int attackButtonY = _paintCardManager.PaintLegendCardDCC(g, legendCard, scale: _scale);
                int descriptionY = AddAttackButtons(legendCard, attackButtonY);
                _paintCardManager.PaintLegendDescription(g, legendCard, descriptionY, scale: _scale);
            }
            else if (Card is WeaponCard weaponCard)
            {
                _paintCardManager.PaintWeaponCardDCC(g, weaponCard, scale: _scale);
            }
            else
            {
                _paintCardManager.PaintAnyOtherCardDCC(g, Card, scale: _scale);
            }
            _paintCardManager.PaintCardBorder(g, scale: _scale);
        }

        // Attack Buttons (Initialized while painting)
        int AddAttackButtons(LegendCard legendCard, int attackButtonY)
        {
            List<Attack> legendAttacks = legendCard.GetAttacks();

            // add Attack buttons
            foreach (Attack attack in legendAttacks)
            {
                // Init. Button
                int damage = attack.AttackModifier + legendCard.Power;
                if (damage <= 0)
                {
                    damage = 0;
                }

                // check if you will get bonus damage
                string damageString;
                int elementalBoost = Attack.CheckElementalDamageBoost(legendCard, attack);
                if (elementalBoost > 0)
                {
                    damageString = $"{damage} Damage + {elementalBoost} Elemental Bonus";
                }
                else
                {
                    damageString = $"{damage} Damage";
                }

                // Weapon Description - check burn amount - check multi hit
                string weaponDescription = attack.WeaponTwo != null
                ? $"{attack.WeaponOneAmount}x {attack.WeaponOne} {GetBurnWeaponEmojis(attack.WeaponOneBurnAmount)} + {attack.WeaponTwoAmount}x {attack.WeaponTwo} {GetBurnWeaponEmojis(attack.WeaponTwoBurnAmount)}"
                : $"{attack.WeaponOneAmount}x {attack.WeaponOne} {GetBurnWeaponEmojis(attack.WeaponOneBurnAmount)}";
                weaponDescription += attack.MultiHit ? "- Hits All" : "";

                // Create the button
                Button attackButton = new Button
                {
                    Text = $"" +
                    $"{attack.Name} ({damageString})\n" + weaponDescription,
                    Location = new Point((Width - (Width - 20)) / 2, attackButtonY),
                    Size = new Size(Width - 20, 50),
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    BackColor = Color.LightGray,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                // Add click event
                attackButton.Click += (sender, e) =>
                {
                    if (_game.ActivePlayer != _game.Me)
                    {
                        MessageBox.Show("It is not your turn!");
                        return;
                    }
                    if (this.OriginalCardControl.Card is LegendCard legend && legend.AttackedThisTurn == true)
                    {
                        MessageBox.Show("This legend already attacked this turn");
                    }
                    else if (this.OriginalCardControl.Card is LegendCard legend2 && legend2.CanAttack == false)
                    {
                        MessageBox.Show("This legend cannot attack right now");
                    }
                    else if (!_game.GetSomeoneIsAttacking())
                    {
                        _game.StartAttack(attack);
                        OriginalCardControl.Enabled = false; // prevents from attacking twice // or attacking yourself
                        Player otherPlayer = _game.GetOtherPlayer(Owner);
                        FRM_Game parentForm = (FRM_Game)this.FindForm();


                        // THESE ATTACKS DON'T ATTACK
                        if (attack.InstaEffect)
                        {
                            // Setup for the message
                            int fieldIndex = Owner.PlayingField.IndexOf(legendCard);

                            // Attack
                            attack.Effect.Invoke(legendCard, null, attack, _game.ActivePlayer, _game); // send this as a msg
                            // Burn Weapons
                            legendCard.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
                            legendCard.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);

                            // Stop Attacking
                            StopAttacking();

                            // Remove Card
                            OnDetailedCardClicked();

                            // The message
                            NETWORK_SendMessage($"STATUS_ATTACK:LEGEND_INDEX:{fieldIndex}:ATTACK:{attack.Name}");
                        }
                        // ATTACK THE PLAYER - This checks if // there are no cards enemy field // not a friendly fire attack // more than 0 damage (otherwise its probably status)
                        else if (otherPlayer.PlayingField.Count == 0 && attack.FriendlyFire == false && AttackCatalogue.CalculateDamage(legendCard, attack) > 0)
                        {
                            // Setup for the message
                            int fieldIndex = Owner.PlayingField.IndexOf(legendCard);

                            // The thing
                            AttackThePlayer(legendCard, otherPlayer, attack); // send this as a msg

                            // The message
                            NETWORK_SendMessage($"ATTACK_PLAYER:LEGEND_INDEX:{fieldIndex}:ATTACK:{attack.Name}:TARGET_PLAYER_IS_HOST:{otherPlayer.IsHost}");
                        }
                        // PREPARE FOR A LEGEND ATTACK
                        else if (attack.FriendlyFire == true)
                        {
                            ZoneControl myPlayingFieldZone = parentForm.UI.GetMyZone(ZoneTypes.PlayingField, Owner);
                            foreach (CardControl cardControl in myPlayingFieldZone.CardsControls)
                            {
                                cardControl.CardClicked += OnClickCardControlDuringAttack; // Subscribing to the click event
                            }
                            // enable friendly cards
                            _game.EnableCardsInZone(Owner, ZoneTypes.PlayingField, true);
                        }
                        else if (otherPlayer.PlayingField.Count > 0)
                        {
                            if (attack.MultiHit)
                            {
                                // Attack Everyone
                                ZoneControl opponentPlayingFieldZone = parentForm.UI.GetMyZone(ZoneTypes.PlayingField, otherPlayer);
                                foreach (CardControl cardControl in opponentPlayingFieldZone.CardsControls.ToList())
                                {
                                    // Setup for the message
                                    int fieldIndex = Owner.PlayingField.IndexOf(legendCard);

                                    // Get enemy field index
                                    int enemyFieldIndex;
                                    if (attack.FriendlyFire)
                                    {
                                        enemyFieldIndex = _game.ActivePlayer.PlayingField.IndexOf(cardControl.Card);
                                    }
                                    else
                                    {
                                        enemyFieldIndex = _game.InactivePlayer.PlayingField.IndexOf(cardControl.Card);
                                    }

                                    // Start Attacking
                                    _game.StartAttack(attack);
                                    AttackLegend(legendCard, cardControl);

                                    // send msg
                                    NETWORK_SendMessage($"ATTACK_LEGEND:LEGEND_INDEX:{fieldIndex}:ATTACK:{attack.Name}:TARGET_LEGEND_INDEX:{enemyFieldIndex}");
                                }
                            }
                            else
                            {
                                // Let the enemy cards know that we are attacking
                                // Enable enemy cards to be able to be clicked to take damage
                                if (attack.FriendlyFire == false)
                                {
                                    ZoneControl opponentPlayingFieldZone = parentForm.UI.GetMyZone(ZoneTypes.PlayingField, otherPlayer);
                                    foreach (CardControl cardControl in opponentPlayingFieldZone.CardsControls)
                                    {
                                        cardControl.CardClicked += OnClickCardControlDuringAttack; // Subscribing to the click event
                                    }
                                }

                                // now the player will click on an opposing card and attack it
                                _game.StartAttack(attack);
                            }
                        }
                        foreach (Button attackButton in _attackButtons.ToList()) // wtf why everyone's attacks
                        {
                            attackButton.Enabled = false;
                        }
                        attackButton.BackColor = Color.LightGreen;


                        // Recoil Check dead
                        if (attack.Recoil > 0)
                        {
                            this.OriginalCardControl.CheckIfDead();
                        }
                    }
                    else
                    {
                        OnDetailedCardClicked();
                        _game.StopAttack();
                    }

                };
                _attackButtons.Add(attackButton);
                // Add To UI
                Controls.Add(attackButton);

                // ENABLE ATTACK BUTTONS IF ABLE TO PLAY
                EnableAttackButton(legendCard, attack, attackButton);

                // Change Y for next attack button
                attackButtonY += 45;
            }

            return attackButtonY;

            void EnableAttackButton(LegendCard legendCard, Attack attack, Button attackButton)
            {
                // only enable them if this is your legend && if the card is on the playing field
                if (Owner == _game.ActivePlayer && legendCard.OnPlayingField)
                {
                    // Assume the attack can be played unless we find a reason it can't
                    bool canPlayAttack = true;

                    // Check if WeaponOne requirement is met
                    int weaponOneCount = CountWeaponCards(legendCard, attack.WeaponOne, attack.WeaponOneAmount);
                    if (weaponOneCount < attack.WeaponOneAmount)
                    {
                        canPlayAttack = false; // WeaponOne requirement is not met
                    }

                    // Check if WeaponTwo requirement is met (only if WeaponTwo is not null)
                    if (attack.WeaponTwo != null)
                    {
                        int weaponTwoCount = CountWeaponCards(legendCard, attack.WeaponTwo, (int)attack.WeaponTwoAmount);
                        if (weaponTwoCount < attack.WeaponTwoAmount)
                        {
                            canPlayAttack = false; // WeaponTwo requirement is not met
                        }
                    }

                    // Enable or disable the attack button based on whether all conditions are met
                    attackButton.Enabled = canPlayAttack;
                }
                else
                {
                    attackButton.Enabled = false;
                }
            }
        }
        string GetBurnWeaponEmojis(int nBurn)
        {
            string emojis = "";
            for (int i = 0; i < nBurn; i++)
            {
                emojis += "🔥";
            }
            return emojis;
        }
        int CountWeaponCards(LegendCard legendCard, Weapons? weaponType, int weaponAmount)
        {
            int weaponCount = 0;

            if (weaponType == Weapons.Any)
            {
                weaponCount = legendCard.StackedCards
                    .Count(card => card is WeaponCard);
            }
            else
            {
                weaponCount = legendCard.StackedCards
                    .Count(card => card is WeaponCard wc && wc.Weapon == weaponType);
            }

            return weaponCount >= weaponAmount ? weaponAmount : 0;
        }


        // Combat
        public void AttackThePlayer(LegendCard legend, Player otherPlayer, Attack attack)
        {
            // Attack
            attack.Effect.Invoke(legend, otherPlayer, attack, _game.ActivePlayer, _game); // send attack name? // attacking legend card index
            // Burn Weapons
            legend.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
            legend.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);
            // update player health
            UI_UpdatePlayerInformation(otherPlayer);
            // Notify
            MessageBox.Show($"{otherPlayer.Name} just took damage");
            // Check if dead
            if (otherPlayer.Health <= 0)
            {
                MessageBox.Show($"{otherPlayer.Name} has been defeated");
            }
            // Stop Attacking
            StopAttacking();
            // Remove Card
            OnDetailedCardClicked();
            // Check if i died
            this.OriginalCardControl.CheckIfDead();
        }
        void AttackLegend(LegendCard legend, CardControl enemyCardControl)
        {
            LegendCard targetLegend = (LegendCard)enemyCardControl.Card;

            // Apply the Damage
            Attack attack = _game.GetSelectedAttack();
            attack.Effect.Invoke(legend, targetLegend, _game.GetSelectedAttack(), _game.ActivePlayer, _game);
            // Burn Weapons
            legend.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
            legend.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);

            // CHECK IF DEAD
            enemyCardControl.CheckIfDead();
            enemyCardControl.Invalidate();
            enemyCardControl.Update();

            // Stop Attacking
            StopAttacking();

            // Remove Detailed card off screen
            OnDetailedCardClicked();

            // Check if i died
            this.OriginalCardControl.CheckIfDead();
        }


        // Removes from this UI
        void RemoveThisFromScreen(Form parentForm)
        {
            // 1️⃣ Unsubscribe from CardClicked events to avoid memory leaks
            FRM_Game playingField = parentForm as FRM_Game;
            if (playingField != null)
            {
                foreach (Player player in Players)
                {
                    ZoneControl opponentZone = playingField.UI.GetMyZone(ZoneTypes.PlayingField, player);
                    foreach (CardControl cardControl in opponentZone.CardsControls)
                    {
                        cardControl.CardClicked -= OnClickCardControlDuringAttack;
                    }
                }
            }


            // Remove stacked weapons
            if (Card is LegendCard legendCard)
            {
                foreach (DetailedCardControl dcc in WeaponCardControls.ToList())
                {
                    // Logically
                    WeaponCardControls.Remove(dcc);
                    // Visually
                    parentForm.Controls.Remove(dcc);
                    dcc.Dispose();
                }
            }


            // Remove this DetailedCardControl
            parentForm.Controls.Remove(this);
            this.Invalidate();
            this.Update();
            this.Dispose();
        }
        void StopAttacking()
        {
            LegendCard legend = (LegendCard)this.OriginalCardControl.Card;
            legend.Attack();
            _game.StopAttack();
        }


        // Events
        public void OnDetailedCardClicked()
        {
            if (_isRemoved) return; // Already removed, skip
            _isRemoved = true;

            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                RemoveThisFromScreen(parentForm);
            }

            if (_game.GetSomeoneIsAttacking())
            {
                OriginalCardControl.Enabled = true;
                _game.StopAttack();
            }
        }
        void OnClickCardControlDuringAttack(CardControl clickedCard)
        {
            if (_game.GetSomeoneIsAttacking())
            {
                // Setup the message
                Attack attack = _game.GetSelectedAttack();
                int fieldIndex = Owner.PlayingField.IndexOf(this.Card);
                int enemyFieldIndex;
                if (attack.FriendlyFire)
                {
                    enemyFieldIndex = _game.ActivePlayer.PlayingField.IndexOf(clickedCard.Card);
                }
                else
                {
                    enemyFieldIndex = _game.InactivePlayer.PlayingField.IndexOf(clickedCard.Card);
                }

                // the actual attack
                AttackLegend((LegendCard)this.Card, clickedCard);

                // send the message
                NETWORK_SendMessage($"ATTACK_LEGEND:LEGEND_INDEX:{fieldIndex}:ATTACK:{attack.Name}:TARGET_LEGEND_INDEX:{enemyFieldIndex}");
            }
        } // COMMUNICATION FUNCTION
    }
}
