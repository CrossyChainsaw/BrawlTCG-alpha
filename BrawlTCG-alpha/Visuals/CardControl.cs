using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Logic.Cards;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D; // Required for GraphicsPath

namespace BrawlTCG_alpha.Visuals
{
    internal class CardControl : Control
    {
        // Card properties (you can extend these with more data)
        public Card Card { get; private set; }
        public bool IsOpen { get; private set; }
        public Player Owner { get; private set; }
        Image _backSideImage = Properties.Resources.BrawlLogo;
        public event Func<Task<bool>> CardReleased;
        // Variables for dragging
        bool _isDragging = false;
        Point _mouseOffset;
        Point _locationBeforeDragging;

        // Methods
        public CardControl(Card card, bool isOpen = false, Player owner = null)
        {
            IsOpen = isOpen;
            Card = card;
            Size = new Size(150, 200); // Default card size
            BackColor = Color.White; // Background color of the card
            Font = new Font("Arial", 12, FontStyle.Bold);
            // You can customize the behavior when clicked, for example
            this.Click += (sender, e) =>
            {
                OnCardClicked();
            };
            Owner = owner;
            // Enable dragging when the user clicks on the card
            this.MouseDown += (sender, e) =>
            {
                // Start dragging
                _isDragging = true;
                _mouseOffset = e.Location;  // Remember where the mouse was clicked relative to the card
                _locationBeforeDragging = this.Location;
            };

            // Move the card while dragging
            this.MouseMove += (sender, e) =>
            {
                if (_isDragging)
                {
                    // Calculate the new location based on the mouse movement
                    this.Left += e.X - _mouseOffset.X;
                    this.Top += e.Y - _mouseOffset.Y;
                }
            };

            // Stop dragging when the mouse is released
            this.MouseUp += async (sender, e) =>
            {
                _isDragging = false;
                bool cardPlayed = await CardReleased.Invoke();
                if (cardPlayed)
                {

                }
                else
                {
                    this.Location = _locationBeforeDragging;
                }
            };
        }

        // Paint method to draw the card
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Draw card background

            if (IsOpen)
            {
                if (Card is LegendCard legendCard)
                {

                    Brush brush = new SolidBrush(legendCard.CardColor);
                    g.FillRectangle(brush, 0, 0, Width, Height);

                    // Define the aspect ratio (4:3 for LegendCard)
                    float aspectRatio = 4f / 3f;
                    int maxWidth = Width - 20;
                    int maxHeight = Height - 60;

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
                    g.SetClip(roundedImagePath);   // Apply the rounded clip

                    // Draw the image with rounded corners
                    g.DrawImage(legendCard.Image, new Rectangle(x, y, newWidth, newHeight));

                    // Reset the clipping region to its default state
                    g.ResetClip();

                    // Draw text elements
                    g.DrawString(legendCard.Name, Font, Brushes.Black, new PointF(5, 5));
                    g.DrawString(legendCard.Cost.ToString(), Font, Brushes.Black, new PointF(Width - 20, Height - 25));
                    g.DrawString($"HP {legendCard.CurrentHP}/{legendCard.HitPoints}", Font, Brushes.Black, new PointF(5, Height - 71));
                    g.DrawString($"Att {legendCard.Power}", Font, Brushes.Black, new PointF(5, Height - 48));

                }
                else
                {
                    Brush brush = new SolidBrush(Card.CardColor);
                    g.FillRectangle(brush, 0, 0, Width, Height);

                    g.DrawImage(Card.Image, new Rectangle(10, 30, Width - 20, Height - 60));
                    g.DrawString(Card.Name, Font, Brushes.Black, new PointF(5, 5));
                    g.DrawString(Card.Cost.ToString(), Font, Brushes.Black, new PointF(Width - 20, Height - 25));
                }
            }
            else
            {
                g.FillRectangle(Brushes.LightBlue, 0, 0, Width, Height);
                g.DrawImage(_backSideImage, new Rectangle(10, 30, Width - 20, Height - 60));
            }


            // Draw the inner border (1px thick) within the card
            int borderThickness = 3;
            g.DrawRectangle(new Pen(Color.Black, borderThickness), 0, 0, Width - 2, Height - 2);
        }

        // Example method to handle card click event
        public void FlipCard()
        {
            IsOpen = !IsOpen;
            Invalidate();
        }
        private void OnCardClicked()
        {
            //MessageBox.Show($"Card {Card.Name} clicked!");
        }
    }
}
