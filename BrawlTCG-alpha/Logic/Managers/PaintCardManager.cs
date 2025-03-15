using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Visuals;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BrawlTCG_alpha.Logic.Managers
{
    internal class PaintCardManager
    {
        // Variables
        const int CARD_WIDTH = 150;
        const int CARD_HEIGHT = 200;
        public Image BackSideImage { get; } = Properties.Resources.BrawlLogo;
        public Font Font { get; } = new Font("Arial", 12, FontStyle.Bold);


        // Paint - CardControl
        public void PaintCardCC(PaintEventArgs e, Card card)
        {
            Graphics g = e.Graphics;

            if (card.IsOpen)
            {
                if (card is LegendCard legendCard)
                {
                    PaintLegendCardCC(g, legendCard);
                }
                else if (card is StageCard stageCard)
                {
                    PaintStageCardCC(g, stageCard);
                }
                else
                {
                    PaintAnyOtherCardCC(g, card);
                }
            }
            else
            {
                PaintCardBackSideCC(g);
            }
            PaintCardBorder(g);
        }
        void PaintLegendCardCC(Graphics g, Card card)
        {
            LegendCard legendCard = (LegendCard)card;

            Brush brush = new SolidBrush(legendCard.CardColor);
            g.FillRectangle(brush, 0, 0, CARD_WIDTH, CARD_HEIGHT);

            // Define the aspect ratio (4:3 for LegendCard)
            float aspectRatio = 4f / 3f;
            int maxWidth = CARD_WIDTH - 20;
            int maxHeight = CARD_HEIGHT - 60;

            int newWidth = maxWidth;
            int newHeight = (int)(newWidth / aspectRatio);

            // Adjust if the height exceeds the available space
            if (newHeight > maxHeight)
            {
                newHeight = maxHeight;
                newWidth = (int)(newHeight * aspectRatio);
            }

            // Center the image
            int x = 10 + (maxWidth - newWidth) / 2;
            int y = 10 + (maxHeight - newHeight) / 2;

            // Rounded corners for the image
            int cornerRadius = 15; // Adjust for more or less rounding
            GraphicsPath roundedImagePath = new GraphicsPath();
            roundedImagePath.AddArc(x, y, cornerRadius, cornerRadius, 180, 90); // Top-left
            roundedImagePath.AddArc(x + newWidth - cornerRadius, y, cornerRadius, cornerRadius, 270, 90); // Top-right
            roundedImagePath.AddArc(x + newWidth - cornerRadius, y + newHeight - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Bottom-right
            roundedImagePath.AddArc(x, y + newHeight - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Bottom-left
            roundedImagePath.CloseFigure();

            // Clip the drawing area to the rounded rectangle
            g.SetClip(roundedImagePath);

            // Draw the image with rounded corners
            g.DrawImage(legendCard.Image, new Rectangle(x, y, newWidth, newHeight));

            // Reset the clipping region to its default state
            g.ResetClip();

            // Draw text elements
            Brush textBrush = new SolidBrush(card.TextColor);
            g.DrawString(legendCard.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(legendCard.Cost.ToString(), Font, textBrush, new PointF(CARD_WIDTH - 20, CARD_HEIGHT - 25));
            g.DrawString($"HP {legendCard.CurrentHP}/{legendCard.BaseHealth}", Font, textBrush, new PointF(5, CARD_HEIGHT - 71));
            g.DrawString($"Att {legendCard.Power}", Font, textBrush, new PointF(5, CARD_HEIGHT - 48));
        }
        void PaintStageCardCC(Graphics g, Card card)
        {
            StageCard stageCard = (StageCard)card;

            Brush brush = new SolidBrush(stageCard.CardColor);
            g.FillRectangle(brush, 0, 0, CARD_WIDTH, CARD_HEIGHT);

            // Define the aspect ratio (4:3 for LegendCard)
            float aspectRatio = 4f / 3f;
            int maxWidth = CARD_WIDTH - 20;
            int maxHeight = CARD_HEIGHT - 60;

            int newWidth = maxWidth;
            int newHeight = (int)(newWidth / aspectRatio);

            // Adjust if the height exceeds the available space
            if (newHeight > maxHeight)
            {
                newHeight = maxHeight;
                newWidth = (int)(newHeight * aspectRatio);
            }

            // Center the image
            int x = 10 + (maxWidth - newWidth) / 2;
            int y = 10 + (maxHeight - newHeight) / 2;

            // Rounded corners for the image
            int cornerRadius = 15; // Adjust for more or less rounding
            GraphicsPath roundedImagePath = new GraphicsPath();
            roundedImagePath.AddArc(x, y, cornerRadius, cornerRadius, 180, 90); // Top-left
            roundedImagePath.AddArc(x + newWidth - cornerRadius, y, cornerRadius, cornerRadius, 270, 90); // Top-right
            roundedImagePath.AddArc(x + newWidth - cornerRadius, y + newHeight - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Bottom-right
            roundedImagePath.AddArc(x, y + newHeight - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Bottom-left
            roundedImagePath.CloseFigure();

            // Clip the drawing area to the rounded rectangle
            g.SetClip(roundedImagePath);

            // Draw the image with rounded corners
            g.DrawImage(stageCard.Image, new Rectangle(x, y, newWidth, newHeight));

            // Reset the clipping region to its default state
            g.ResetClip();

            // Draw text elements
            Brush textBrush = new SolidBrush(card.TextColor);
            g.DrawString(stageCard.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(stageCard.Cost.ToString(), Font, textBrush, new PointF(CARD_WIDTH - 20, CARD_HEIGHT - 25));
        }
        void PaintAnyOtherCardCC(Graphics g, Card card)
        {
            Brush cardBrush = new SolidBrush(card.CardColor);
            g.FillRectangle(cardBrush, 0, 0, CARD_WIDTH, CARD_HEIGHT);
            g.DrawImage(card.Image, new Rectangle(10, 30, CARD_WIDTH - 20, CARD_HEIGHT - 60));
            Brush textBrush = new SolidBrush(card.TextColor);
            g.DrawString(card.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(card.Cost.ToString(), Font, textBrush, new PointF(CARD_WIDTH - 20, CARD_HEIGHT - 25));
        }
        void PaintCardBackSideCC(Graphics g)
        {
            g.FillRectangle(Brushes.LightBlue, 0, 0, CARD_WIDTH, CARD_HEIGHT);
            g.DrawImage(BackSideImage, new Rectangle(10, 30, CARD_WIDTH - 20, CARD_HEIGHT - 60));
        }


        // Paint - DetailedCardControl
        public int PaintLegendCardDCC(Graphics g, Card card, float scale)
        {
            LegendCard legendCard = (LegendCard)card;
            int width = (int)(CARD_WIDTH * scale);
            int height = (int)(CARD_HEIGHT * scale);

            Brush brush = new SolidBrush(legendCard.CardColor);
            g.FillRectangle(brush, 0, 0, width, height);

            float aspectRatio = 4f / 3f;
            int maxWidth = width - 20;
            int availableHeight = height - 60;
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


            Brush textBrush = new SolidBrush(card.TextColor);

            g.DrawString(legendCard.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(legendCard.Cost.ToString(), Font, textBrush, new PointF(width - 20, height - 25));
            g.DrawString($"HP {legendCard.CurrentHP}/{legendCard.BaseHealth}", Font, textBrush, new PointF(width - 100, 5));
            SizeF attSize = g.MeasureString($"Att {legendCard.Power}", Font);
            g.DrawString($"Att {legendCard.Power}", Font, textBrush, new PointF((width - attSize.Width) / 2, 5));

            int attackButtonY = y + newHeight + 10;
            return attackButtonY;
        }
        public void PaintAnyOtherCardDCC(Graphics g, Card card, float scale)
        {
            int width = (int)(CARD_WIDTH * scale);
            int height = (int)(CARD_HEIGHT * scale);

            Brush cardBrush = new SolidBrush(card.CardColor);
            g.FillRectangle(cardBrush, 0, 0, width, height);

            // Keep the original proportions of the image
            float aspectRatio = (float)card.Image.Width / card.Image.Height;

            // Define the maximum size for the image
            int maxWidth = width - 20;  // Padding of 10 on each side (left/right)
            int maxHeight = height - 60; // Padding of 30 (top/bottom)

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
            int x = (width - imageWidth) / 2;
            int y = 30;

            // Draw the image (scaled to fit within the available space)
            g.DrawImage(card.Image, new Rectangle(x, y, imageWidth, imageHeight));

            // Calculate space for the description text
            int descriptionTop = y + imageHeight + 5;  // 5px padding below the image
            int descriptionWidth = width - 20;         // Padding on the left/right

            // Draw the card's description (aligned to the left)
            Brush textBrush = new SolidBrush(card.TextColor);
            StringFormat textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Near; // Align to the left

            // You can adjust the font size or layout based on the description length
            g.DrawString(card.Description, Font, textBrush, new Rectangle(10, descriptionTop, descriptionWidth, height - descriptionTop - 10), textFormat);

            // Draw the card's name and cost (as before)
            g.DrawString(card.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(card.Cost.ToString(), Font, textBrush, new PointF(width - 20, height - 25));
        }
        public void PaintWeaponCardDCC(Graphics g, WeaponCard card, float scale = 1)
        {
            int width = (int)(CARD_WIDTH * scale);
            int height = (int)(CARD_HEIGHT * scale);

            Brush cardBrush = new SolidBrush(card.CardColor);
            g.FillRectangle(cardBrush, 0, 0, width, height);

            // Keep the original proportions of the image
            float aspectRatio = (float)card.Image.Width / card.Image.Height;

            // Define the maximum size for the image
            int maxWidth = width - 60;  // Padding of 10 on each side (left/right) // make 60
            int maxHeight = height - 60; // Padding of 30 (top/bottom)

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
            int x = (width - imageWidth) / 2;
            int y = (height - imageHeight) / 2;

            // Draw the image (scaled to fit within the available space)
            g.DrawImage(card.Image, new Rectangle(x, y, imageWidth, imageHeight));

            // Calculate space for the description text
            int descriptionTop = y + imageHeight + 5;  // 5px padding below the image
            int descriptionWidth = width - 30;         // Padding on the left/right

            // Draw the card's description (aligned to the left)
            Brush textBrush = new SolidBrush(card.TextColor);
            StringFormat textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Near; // Align to the left

            // You can adjust the font size or layout based on the description length
            g.DrawString(card.Description, Font, textBrush, new Rectangle(10, descriptionTop, descriptionWidth, height - descriptionTop - 10), textFormat);


            // Draw the card's name and cost (as before)
            g.DrawString(card.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(card.Cost.ToString(), Font, textBrush, new PointF(width - 20, height - 25));
        }
        public void PaintLegendDescription(Graphics g, LegendCard legend, int y, float scale)
        {
            int width = (int)(CARD_WIDTH * scale);
            int height = (int)(CARD_HEIGHT * scale);
            legend.Description = Card.GenerateEffectDescription(legend);

            // Description
            if (!string.IsNullOrEmpty(legend.Description))
            {
                Brush textBrush = new SolidBrush(legend.TextColor);

                // Determine description position
                int descriptionTop = y + 10;
                int descriptionWidth = width - 20;       // 10px padding left/right
                int descriptionHeight = height - descriptionTop - 10; // Remaining height

                // Draw Description
                StringFormat textFormat = new StringFormat { Alignment = StringAlignment.Near };
                g.DrawString(
                    legend.Description,
                    Font,
                    textBrush,
                    new Rectangle(10, descriptionTop, descriptionWidth, descriptionHeight),
                    textFormat
                );
            }
        }


        // Paint - Shared
        public void PaintCardBorder(Graphics g, float scale = 1)
        {
            int borderThickness = 3;
            g.DrawRectangle(new Pen(Color.Black, borderThickness), 0, 0, (CARD_WIDTH * scale) - 2, (CARD_HEIGHT * scale) - 2);
        }


    }
}
