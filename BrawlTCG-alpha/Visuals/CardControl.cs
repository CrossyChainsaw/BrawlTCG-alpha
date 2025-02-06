using BrawlTCG_alpha.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Brush brush = new SolidBrush(Card.CardColor);
                g.FillRectangle(brush, 0, 0, Width, Height);
                g.DrawImage(Card.Image, new Rectangle(10, 30, Width - 20, Height - 60));
                g.DrawString(Card.Name, Font, Brushes.Black, new PointF(5, 5));
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
