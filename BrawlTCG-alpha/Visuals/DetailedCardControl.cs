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
        public Card Card { get; private set; }
        public List<CardControl> CardsControls { get; internal set; }
        private List<Button> attackButtons;

        public DetailedCardControl(Card card)
        {
            Card = card;
            Size = new Size(150, 200);
            BackColor = card.CardColor;
            ForeColor = card.TextColor;
            Font = new Font("Arial", 12, FontStyle.Bold);
            this.Click += (sender, e) => OnCardClicked();
            CardsControls = new List<CardControl>();
            attackButtons = new List<Button>();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (Card is LegendCard legendCard)
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
                g.DrawString($"HP {legendCard.CurrentHP}/{legendCard.HitPoints}", Font, textBrush, new PointF(Width - 100, 5));
                SizeF attSize = g.MeasureString($"Att {legendCard.Power}", Font);
                g.DrawString($"Att {legendCard.Power}", Font, textBrush, new PointF((Width - attSize.Width) / 2, 5));

                List<Attack> legendAttacks = legendCard.GetAttacks();
                int attackButtonY = y + newHeight + 20;

                // add Attack buttons
                foreach (Attack attack in legendAttacks)
                {
                    // Init. Button
                    Button attackButton = new Button
                    {
                        Text = $"{attack.Name} ({attack.AttackModifier + legendCard.Power} Damage)\n{(attack.WeaponTwo != null ? $"{attack.WeaponOneAmount}x {attack.WeaponOne} + {attack.WeaponTwoAmount}x {attack.WeaponTwo}" : $"{attack.WeaponOneAmount}x {attack.WeaponOne}")}",
                        Location = new Point((Width - (Width - 20)) / 2, attackButtonY),
                        Size = new Size(Width - 20, 50),
                        Font = new Font("Arial", 9, FontStyle.Bold),
                        BackColor = Color.LightGray,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    attackButton.Click += (sender, e) => MessageBox.Show($"Clicked on attack {attack.Name}");
                    attackButtons.Add(attackButton);
                    // Add To UI
                    Controls.Add(attackButton);
                    
                    // Check if the attack can be played
                    // Check weapon one requirement
                    bool canPlayAttack = legendCard.StackedCards.Count(c => c is WeaponCard wc && wc.Weapon == attack.WeaponOne) >= attack.WeaponOneAmount;
                    // Check weapon two requirement if there is 
                    if (attack.WeaponTwo != null)
                    {
                        canPlayAttack &= legendCard.StackedCards.Count(c => c is WeaponCard wc && wc.Weapon == attack.WeaponTwo) >= attack.WeaponTwoAmount;
                    }
                    // Enable or disable the button based on weapon availability
                    attackButton.Enabled = canPlayAttack;

                    // Change Y for next attack button
                    attackButtonY += 50;
                }
            }
            else
            {
                Font font = new Font("Arial", 16, FontStyle.Bold);
                Brush cardBrush = new SolidBrush(Card.CardColor);
                g.FillRectangle(cardBrush, 0, 0, Width, Height);
                g.DrawImage(Card.Image, new Rectangle(10, 30, Width - 20, Height - 60));
                Brush textBrush = new SolidBrush(Card.TextColor);
                g.DrawString(Card.Name, font, textBrush, new PointF(5, 5));
                g.DrawString(Card.Cost.ToString(), font, textBrush, new PointF(Width - 25, Height - 33));
            }

            int borderThickness = 3;
            g.DrawRectangle(new Pen(Color.Black, borderThickness), 0, 0, Width - 2, Height - 2);
        }

        private void OnCardClicked()
        {
            Form parentForm = this.FindForm();
            parentForm.Controls.Remove(this);

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

            this.Dispose();
        }
    }
}
