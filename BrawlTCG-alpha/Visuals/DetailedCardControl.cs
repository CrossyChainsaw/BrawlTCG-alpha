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
        private bool isRemoved = false; // Prevent double removal
        private Game _game;
        private List<Button> attackButtons;
        public bool IsAttacking = false;
        public Card Card { get; private set; }
        public List<CardControl> CardsControls { get; internal set; }

        // Events
        public event Action<Player> UI_UpdatePlayerInformation;
        public event Action<Player> UI_ArrangeCardsInPlayingField;

        public DetailedCardControl(Game game, Card card, Player owner, List<Player> players, CardControl originalCardControl, Action<Player> arrangeCards)
        {
            Card = card;
            Size = new Size(150, 200);
            BackColor = card.CardColor;
            ForeColor = card.TextColor;
            Font = new Font("Arial", 12, FontStyle.Bold);
            this.Click += (sender, e) => OnDetailedCardClicked();
            CardsControls = new List<CardControl>();
            attackButtons = new List<Button>();
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
            Font font = new Font("Arial", 16, FontStyle.Bold);
            Brush cardBrush = new SolidBrush(Card.CardColor);
            g.FillRectangle(cardBrush, 0, 0, Width, Height);
            g.DrawImage(Card.Image, new Rectangle(10, 30, Width - 20, Height - 60));
            Brush textBrush = new SolidBrush(Card.TextColor);
            g.DrawString(Card.Name, font, textBrush, new PointF(5, 5));
            g.DrawString(Card.Cost.ToString(), font, textBrush, new PointF(Width - 25, Height - 33));
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

                Button attackButton = new Button
                {
                    Text = $"{attack.Name} ({damageString})\n{(attack.WeaponTwo != null ? $"{attack.WeaponOneAmount}x {attack.WeaponOne} + {attack.WeaponTwoAmount}x {attack.WeaponTwo}" : $"{attack.WeaponOneAmount}x {attack.WeaponOne}")}",
                    Location = new Point((Width - (Width - 20)) / 2, attackButtonY),
                    Size = new Size(Width - 20, 50),
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    BackColor = Color.LightGray,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                attackButton.Click += (sender, e) =>
                {
                    if (!_game.SomeoneIsAttacking)
                    {
                        _game.StartAttack(attack);
                        OriginalCardControl.Enabled = false; // prevents from attacking twice
                        Player otherPlayer = _game.GetOtherPlayer(Owner);

                        // ATTACK THE PLAYER
                        if (otherPlayer.PlayingField.Count == 0)
                        {
                            AttackThePlayer(legendCard, otherPlayer, attack);
                            _game.StopAttack();
                        }
                        // PREPARE FOR A LEGEND ATTACK
                        else
                        {
                            // Let the enemy cards know that we are attacking
                            // Enable enemy cards to be able to be clicked to take damage
                            FRM_PlayingField parentForm = (FRM_PlayingField)this.FindForm();
                            ZoneControl opponentPlayingFieldZone = parentForm.GetMyZone(ZoneTypes.PlayingField, otherPlayer);
                            foreach (CardControl cardControl in opponentPlayingFieldZone.CardsControls)
                            {
                                cardControl.CardClicked += OnEnemyCardControlClicked; // Subscribing to the click event
                            }

                            // now the player will click on an opposing card and attack it
                            _game.StartAttack(attack);
                        }
                        foreach (Button attackButton in attackButtons.ToList())
                        {
                            attackButton.Enabled = false;
                        }
                        attackButton.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        OnDetailedCardClicked();
                        _game.StopAttack();
                    }
                };
                attackButtons.Add(attackButton);
                // Add To UI
                Controls.Add(attackButton);



                // ENABLE ATTACK BUTTONS IF ABLE TO PLAY
                EnableAttackButton(legendCard, attack, attackButton);

                // Change Y for next attack button
                attackButtonY += 50;
            }
        }
        void EnableAttackButton(LegendCard legendCard, Attack attack, Button attackButton)
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

        private int CountWeaponCards(LegendCard legendCard, Weapons? weaponType, int weaponAmount)
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
        void AttackThePlayer(LegendCard legendCard, Player otherPlayer, Attack attack)
        {
            // Remove Card
            OnDetailedCardClicked();
            // Attack
            attack.Effect.Invoke(legendCard, otherPlayer, attack);
            UI_UpdatePlayerInformation(otherPlayer);
            // Notify
            MessageBox.Show($"{otherPlayer.Name} just took damage");
            // Check if dead
            if (otherPlayer.Health <= 0)
            {
                MessageBox.Show($"{otherPlayer.Name} has been defeated");
            }
        }
        void AttackLegendCard(LegendCard legendCard, CardControl enemyCardControl)
        {
            LegendCard targetLegend = (LegendCard)enemyCardControl.Card;

            // Apply the Damage
            _game.SelectedAttack.Effect.Invoke(legendCard, targetLegend, _game.SelectedAttack);
            enemyCardControl.Invalidate();
            enemyCardControl.Update();

            // Notify someone took damage
            //MessageBox.Show($"{enemyCardControl.Card.Name} just took damage");

            // CHECK IF DEAD
            enemyCardControl.CheckIfDead();

            // Stop Attacking
            _game.StopAttack();
            // Remove Detailed card off screen
            OnDetailedCardClicked();
        }
        int CheckElementalDamageBoost(LegendCard attackingLegend, Attack attack)
        {
            int elementalDamageBoost = 0;
            foreach (WeaponCard weaponCard in attackingLegend.StackedCards)
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
            if (attack.WeaponTwo != null)
            {
                foreach (WeaponCard weaponCard in attackingLegend.StackedCards)
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
            return elementalDamageBoost;
        }

        // Removes from this UI
        void RemoveThisFromScreen(Form parentForm)
        {
            // 1️⃣ Unsubscribe from CardClicked events to avoid memory leaks
            FRM_PlayingField playingField = parentForm as FRM_PlayingField;
            if (playingField != null)
            {
                foreach (Player player in Players)
                {
                    ZoneControl opponentZone = playingField.GetMyZone(ZoneTypes.PlayingField, player);
                    foreach (CardControl cardControl in opponentZone.CardsControls)
                    {
                        cardControl.CardClicked -= OnEnemyCardControlClicked;
                    }
                }
            }

            // 2️⃣ Remove this DetailedCardControl
            parentForm.Controls.Remove(this);
            this.Invalidate();
            this.Update();
            this.Dispose();

            // 3️⃣ Also remove stacked weapons if it's a LegendCard
            if (Card is LegendCard legendCard)
            {
                foreach (Card weaponCard in legendCard.StackedCards)
                {
                    Control controlToRemove = parentForm.Controls
                        .OfType<DetailedCardControl>()
                        .FirstOrDefault(c => c.Card == weaponCard);

                    if (controlToRemove != null)
                    {
                        parentForm.Controls.Remove(controlToRemove);
                        controlToRemove.Dispose();
                    }
                }
            }
        }

        // Events
        void OnEnemyCardControlClicked(CardControl clickedCard)
        {
            _ = this.Controls;
            if (_game.SomeoneIsAttacking)
            {
                AttackLegendCard((LegendCard)this.Card, clickedCard);
            }
        }
        void OnDetailedCardClicked()
        {
            if (isRemoved) return; // Already removed, skip
            isRemoved = true;

            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                RemoveThisFromScreen(parentForm);
            }

            if (_game.SomeoneIsAttacking)
            {
                OriginalCardControl.Enabled = true;
                _game.StopAttack();
            }
        }
    }
}
