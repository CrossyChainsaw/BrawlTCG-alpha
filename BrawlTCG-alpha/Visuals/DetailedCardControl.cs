using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace BrawlTCG_alpha.Visuals
{
    internal class DetailedCardControl : Control
    {
        // Fields
        public Player Owner;
        public List<Player> Players;
        public CardControl OriginalCardControl;
        Game _game;
        List<Button> _attackButtons;
        bool _isRemoved = false;
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
        }


        // Paint Cards
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (Card is LegendCard legendCard)
            {
                int attackButtonY = PaintLegendCard(g, legendCard);
                AddAttackButtons(legendCard, attackButtonY);
            }
            else if (Card is WeaponCard weaponCard)
            {
                PaintWeaponCard(g);
            }
            else
            {
                PaintAnyOtherCard(g);
            }
            PaintBorder(g);
        }
        int PaintLegendCard(Graphics g, LegendCard legendCard)
        {
            Brush brush = new SolidBrush(legendCard.CardColor);
            g.FillRectangle(brush, 0, 0, Width, Height);

            float aspectRatio = 4f / 3f;
            int maxWidth = Width - 20;
            int availableHeight = Height - 60;
            int newWidth = maxWidth;
            int newHeight = (int)(newWidth / aspectRatio);

            if (newHeight > availableHeight)
            {
                newHeight = availableHeight;
                newWidth = (int)(newHeight * aspectRatio);
            }

            int x = 10 + (maxWidth - newWidth) / 2;
            int y = 30;

            int cornerRadius = 15;
            GraphicsPath roundedImagePath = new GraphicsPath();
            roundedImagePath.AddArc(x, y, cornerRadius, cornerRadius, 180, 90);
            roundedImagePath.AddArc(x + newWidth - cornerRadius, y, cornerRadius, cornerRadius, 270, 90);
            roundedImagePath.AddArc(x + newWidth - cornerRadius, y + newHeight - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            roundedImagePath.AddArc(x, y + newHeight - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            roundedImagePath.CloseFigure();

            g.SetClip(roundedImagePath);
            g.DrawImage(legendCard.Image, new Rectangle(x, y, newWidth, newHeight));
            g.ResetClip();

            Brush textBrush = new SolidBrush(Card.TextColor);
            g.DrawString(legendCard.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(legendCard.Cost.ToString(), Font, textBrush, new PointF(Width - 20, Height - 25));
            g.DrawString($"HP {legendCard.CurrentHP}/{legendCard.BaseHealth}", Font, textBrush, new PointF(Width - 100, 5));
            SizeF attSize = g.MeasureString($"Att {legendCard.Power}", Font);
            g.DrawString($"Att {legendCard.Power}", Font, textBrush, new PointF((Width - attSize.Width) / 2, 5));

            List<Attack> legendAttacks = legendCard.GetAttacks();
            int attackButtonY = y + newHeight + 20;
            return attackButtonY;
        }
        void PaintAnyOtherCard(Graphics g)
        {
            Brush cardBrush = new SolidBrush(Card.CardColor);
            g.FillRectangle(cardBrush, 0, 0, Width, Height);

            // Keep the original proportions of the image
            float aspectRatio = (float)Card.Image.Width / Card.Image.Height;

            // Define the maximum size for the image
            int maxWidth = Width - 20;  // Padding of 10 on each side (left/right)
            int maxHeight = Height - 60; // Padding of 30 (top/bottom)

            // Calculate the width and height based on the aspect ratio and the available space
            int imageWidth = maxWidth;
            int imageHeight = (int)(imageWidth / aspectRatio);

            // If the image height exceeds the available space, adjust it
            if (imageHeight > maxHeight)
            {
                imageHeight = maxHeight;
                imageWidth = (int)(imageHeight * aspectRatio);
            }

            // Center the image within the control
            int x = (Width - imageWidth) / 2;
            int y = 30;

            // Draw the image (scaled to fit within the available space)
            g.DrawImage(Card.Image, new Rectangle(x, y, imageWidth, imageHeight));

            // Calculate space for the description text
            int descriptionTop = y + imageHeight + 5;  // 5px padding below the image
            int descriptionWidth = Width - 20;         // Padding on the left/right

            // Draw the card's description (aligned to the left)
            Brush textBrush = new SolidBrush(Card.TextColor);
            StringFormat textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Near; // Align to the left

            // You can adjust the font size or layout based on the description length
            g.DrawString(Card.Description, Font, textBrush, new Rectangle(10, descriptionTop, descriptionWidth, Height - descriptionTop - 10), textFormat);

            // Draw the card's name and cost (as before)
            g.DrawString(Card.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(Card.Cost.ToString(), Font, textBrush, new PointF(Width - 20, Height - 25));
        }
        void PaintWeaponCard(Graphics g)
        {
            Brush cardBrush = new SolidBrush(Card.CardColor);
            g.FillRectangle(cardBrush, 0, 0, Width, Height);

            // Keep the original proportions of the image
            float aspectRatio = (float)Card.Image.Width / Card.Image.Height;

            // Define the maximum size for the image
            int maxWidth = Width - 60;  // Padding of 10 on each side (left/right) // make 60
            int maxHeight = Height - 60; // Padding of 30 (top/bottom)

            // Calculate the width and height based on the aspect ratio and the available space
            int imageWidth = maxWidth;
            int imageHeight = (int)(imageWidth / aspectRatio);

            // If the image height exceeds the available space, adjust it
            if (imageHeight > maxHeight)
            {
                imageHeight = maxHeight;
                imageWidth = (int)(imageHeight * aspectRatio);
            }

            // Center the image within the control
            int x = (Width - imageWidth) / 2;
            int y = (Height - imageHeight) / 2;

            // Draw the image (scaled to fit within the available space)
            g.DrawImage(Card.Image, new Rectangle(x, y, imageWidth, imageHeight));

            // Calculate space for the description text
            int descriptionTop = y + imageHeight + 5;  // 5px padding below the image
            int descriptionWidth = Width - 30;         // Padding on the left/right

            // Draw the card's description (aligned to the left)
            Brush textBrush = new SolidBrush(Card.TextColor);
            StringFormat textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Near; // Align to the left

            // You can adjust the font size or layout based on the description length
            g.DrawString(Card.Description, Font, textBrush, new Rectangle(10, descriptionTop, descriptionWidth, Height - descriptionTop - 10), textFormat);


            // Draw the card's name and cost (as before)
            g.DrawString(Card.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(Card.Cost.ToString(), Font, textBrush, new PointF(Width - 20, Height - 25));
        }
        void PaintBorder(Graphics g)
        {
            int borderThickness = 3;
            g.DrawRectangle(new Pen(Color.Black, borderThickness), 0, 0, Width - 2, Height - 2);
        }
        // Attack Buttons (Initialized while painting)
        void AddAttackButtons(LegendCard legendCard, int attackButtonY)
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
                int elementalBoost = CheckElementalDamageBoost(legendCard, attack);
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
                    if (this.OriginalCardControl.Card is LegendCard legend && legend.AttackedThisTurn == true)
                    {
                        MessageBox.Show("This legend already attacked this turn");
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
                            ZoneControl myPlayingFieldZone = parentForm.GetMyZone(ZoneTypes.PlayingField, Owner);
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
                                ZoneControl opponentPlayingFieldZone = parentForm.GetMyZone(ZoneTypes.PlayingField, otherPlayer);
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
                                    AttackLegendCard(legendCard, cardControl);

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
                                    ZoneControl opponentPlayingFieldZone = parentForm.GetMyZone(ZoneTypes.PlayingField, otherPlayer);
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
                attackButtonY += 50;

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
                string GetBurnWeaponEmojis(int nBurn)
                {
                    string emojis = "";
                    for (int i = 0; i < nBurn; i++)
                    {
                        emojis += "🔥";
                    }
                    return emojis;
                }
            }
        }
        public void AttackThePlayer(LegendCard legendCard, Player otherPlayer, Attack attack)
        {
            // Attack
            attack.Effect.Invoke(legendCard, otherPlayer, attack, _game.ActivePlayer, _game); // send attack name? // attacking legend card index
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


        // Attack
        void AttackLegendCard(LegendCard legendCard, CardControl enemyCardControl)
        {
            LegendCard targetLegend = (LegendCard)enemyCardControl.Card;

            // Apply the Damage
            _game.GetSelectedAttack().Effect.Invoke(legendCard, targetLegend, _game.GetSelectedAttack(), _game.ActivePlayer, _game);
            enemyCardControl.Invalidate();
            enemyCardControl.Update();

            // CHECK IF DEAD
            enemyCardControl.CheckIfDead();

            // Stop Attacking
            StopAttacking();

            // Remove Detailed card off screen
            OnDetailedCardClicked();

            // Check if i died
            this.OriginalCardControl.CheckIfDead();
        }
        int CheckElementalDamageBoost(LegendCard attackingLegend, Attack attack)
        {
            int elementalDamageBoost = 0;
            foreach (Card card in attackingLegend.StackedCards)
            {
                if (card is WeaponCard weaponCard)
                {
                    int requiredMatches = attack.WeaponOneAmount;
                    int foundMatches = 0;
                    if (weaponCard.Weapon == attack.WeaponOne)
                    {
                        if (weaponCard.Element == attackingLegend.Element)
                        {
                            foundMatches++;
                            if (foundMatches == requiredMatches)
                            {
                                elementalDamageBoost += requiredMatches;
                                break;
                            }
                        }
                    }
                }
            }
            if (attack.WeaponTwo != null)
            {
                foreach (Card card in attackingLegend.StackedCards)
                {
                    if (card is WeaponCard weaponCard)
                    {
                        int requiredMatches = (int)attack.WeaponTwoAmount;
                        int foundMatches2 = 0;
                        if (weaponCard.Weapon == attack.WeaponTwo)
                        {
                            if (weaponCard.Element == attackingLegend.Element)
                            {
                                foundMatches2++;
                                if (foundMatches2 == requiredMatches)
                                {
                                    elementalDamageBoost += requiredMatches;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return elementalDamageBoost;
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
                    ZoneControl opponentZone = playingField.GetMyZone(ZoneTypes.PlayingField, player);
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
            legend.AttackedThisTurn = true;
            _game.StopAttack();
        }

        // Events
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
                AttackLegendCard((LegendCard)this.Card, clickedCard);

                // send the message
                NETWORK_SendMessage($"ATTACK_LEGEND:LEGEND_INDEX:{fieldIndex}:ATTACK:{attack.Name}:TARGET_LEGEND_INDEX:{enemyFieldIndex}");
            }
        } // COMMUNICATION FUNCTION
        void OnDetailedCardClicked()
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
    }
}
