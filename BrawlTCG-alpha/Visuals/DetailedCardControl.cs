using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Logic;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Visuals
{
    internal class DetailedCardControl : Control
    {
        public Card Card { get; private set; }
        public List<CardControl> CardsControls { get; internal set; }

        // Methods
        public DetailedCardControl(Card card)
        {
            Card = card;
            Size = new Size(150, 200); // Default card size
            BackColor = card.CardColor; // Background color of the card
            ForeColor = card.TextColor;
            Font = new Font("Arial", 12, FontStyle.Bold);
            // You can customize the behavior when clicked, for example
            this.Click += (sender, e) =>
            {
                OnCardClicked();
            };
            CardsControls = new List<CardControl>();
        }

        // Paint method to draw the card
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (Card is LegendCard legendCard)
            {
                Brush brush = new SolidBrush(legendCard.CardColor);
                g.FillRectangle(brush, 0, 0, Width, Height);

                // Define the aspect ratio (4:3 for LegendCard)
                float aspectRatio = 4f / 3f;
                int maxWidth = Width - 20;
                int availableHeight = Height - 60; // Exclude the bottom margin for text and other info

                int newWidth = maxWidth;
                int newHeight = (int)(newWidth / aspectRatio);

                // Adjust if the height exceeds the available space
                if (newHeight > availableHeight)
                {
                    newHeight = availableHeight;
                    newWidth = (int)(newHeight * aspectRatio);
                }

                // Calculate the vertical position with 20px margin at the top and center the image
                int x = 10 + (maxWidth - newWidth) / 2;
                int y = 20 + (availableHeight - newHeight) / 2; // 20px margin from the top
                y = 30;

                // Rounded corners for the image
                int cornerRadius = 15; // Adjust for more or less rounding
                GraphicsPath roundedImagePath = new GraphicsPath();
                roundedImagePath.AddArc(x, y, cornerRadius, cornerRadius, 180, 90); // Top-left
                roundedImagePath.AddArc(x + newWidth - cornerRadius, y, cornerRadius, cornerRadius, 270, 90); // Top-right
                roundedImagePath.AddArc(x + newWidth - cornerRadius, y + newHeight - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Bottom-right
                roundedImagePath.AddArc(x, y + newHeight - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Bottom-left
                roundedImagePath.CloseFigure();

                // Clip the drawing area to the rounded rectangle
                g.SetClip(roundedImagePath);   // Apply the rounded clip

                // Draw the image with rounded corners
                g.DrawImage(legendCard.Image, new Rectangle(x, y, newWidth, newHeight));

                // Reset the clipping region to its default state
                g.ResetClip();

                // Draw text elements
                Brush textBrush = new SolidBrush(Card.TextColor);
                g.DrawString($"{legendCard.Name}", Font, textBrush, new PointF(5, 5));
                g.DrawString(legendCard.Cost.ToString(), Font, textBrush, new PointF(Width - 20, Height - 25));
                g.DrawString($"HP {legendCard.CurrentHP}/{legendCard.HitPoints}", Font, textBrush, new PointF(Width - 100, 5)); // Adjust for padding and alignment
                SizeF attSize = g.MeasureString($"Att {legendCard.Power}", Font);
                g.DrawString($"Att {legendCard.Power}", Font, textBrush, new PointF((Width - attSize.Width) / 2, 5)); // Adjusted for centering


                // Render the Attacks on the card
                List<Attack> legendAttacks = legendCard.GetAttacks();
                int attackTextY = y + newHeight + 20; // Starting Y position: 20px below the image

                foreach (Attack attack in legendAttacks)
                {
                    string attackText = $"{attack.Name}: {attack.WeaponOne} ({attack.WeaponOneAmount})";

                    // Include WeaponTwo if it exists
                    if (attack.WeaponTwo != null && attack.WeaponTwoAmount != null)
                    {
                        attackText += $" & {attack.WeaponTwo} ({attack.WeaponTwoAmount})";
                    }

                    // Draw the attack text
                    g.DrawString(attackText, Font, textBrush, new PointF(10, attackTextY));

                    // Move to the next line, 20px below
                    attackTextY += 20;
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



            //// Draw the inner border (1px thick) within the card
            int borderThickness = 3;
            g.DrawRectangle(new Pen(Color.Black, borderThickness), 0, 0, Width - 2, Height - 2);
        }
        private void OnCardClicked()
        {
            // Find the parent form
            Form parentForm = this.FindForm();

            // Remove the DetailedCardControl for the legend card
            parentForm.Controls.Remove(this);

            // If the card is a LegendCard, we need to remove the related weapon cards
            if (Card is LegendCard legendCard)
            {
                // Get the list of stacked weapon cards
                List<Card> stackedCards = legendCard.StackedCards;

                // Iterate over each stacked card and remove the corresponding DetailedCardControl
                foreach (Card weaponCard in stackedCards)
                {
                    // Find the control corresponding to the weapon card
                    Control controlToRemove = parentForm.Controls
                        .OfType<DetailedCardControl>()
                        .FirstOrDefault(c => c.Card == weaponCard);

                    // Remove the control if it is found
                    if (controlToRemove != null)
                    {
                        parentForm.Controls.Remove(controlToRemove);
                        controlToRemove.Dispose(); // Optionally dispose of the control to free resources
                    }
                }
            }

            // Optionally, you can dispose of the card control itself
            this.Dispose();
        }

    }
}
