#pragma warning disable CS8602 // Dereference of a possibly null reference.
using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Logic.Cards;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Visuals
{
    internal class CardControl : Control
    {
        // Fields
        public Card Card { get; private set; }
        public List<CardControl> CardsControls { get; internal set; }
        public Player? Owner { get; private set; }
        public List<Player> Players { get; internal set; }
        public event Func<Task<bool>>? CardReleased;

        Attack _enemyAttack;
        LegendCard _enemyLegend;
        DetailedCardControl _enemyDetailedCardControl;

        Image _backSideImage = Properties.Resources.BrawlLogo;
        public event Action<CardControl> CardClicked; // Event to notify when this card is clicked

        public event Action<Player> UI_ArrangeCardsInPlayingField;
        Game _game;

        // Variables for dragging
        const int CARD_WIDTH = 150;
        const int CARD_HEIGHT = 200;
        bool _isDragging = false;
        bool _mouseMoved = false;
        bool _canDrag = true;
        Point _mouseOffset;
        Point _locationBeforeDragging;

        // Methods
        public CardControl(Game game, Card card, Action<Player> UI_arrangeCardsFunction, bool isOpen = false, Player? owner = null, List<Player> players = null)
        {
            Card = card;
            this.Card.IsOpen = isOpen;
            Size = new Size(CARD_WIDTH, CARD_HEIGHT); // Default card size
            BackColor = Color.White; // Background color of the card
            Font = new Font("Arial", 12, FontStyle.Bold);
            Owner = owner;
            CardsControls = new List<CardControl>();
            Players = players;
            _game = game;
            // Delegates
            UI_ArrangeCardsInPlayingField += UI_arrangeCardsFunction;
            // Events
            this.Click += (sender, e) =>
            {
                OnCardClicked();
            };
            this.MouseDown += (sender, e) =>
            {
                if (_canDrag)
                {
                    // Start dragging
                    _isDragging = true;
                    _mouseOffset = e.Location;  // Remember where the mouse was clicked relative to the card
                    _locationBeforeDragging = this.Location;
                }
            };
            this.MouseMove += (sender, e) =>
            {
                if (_isDragging)
                {
                    _mouseMoved = true;
                    // Calculate the new location based on the mouse movement
                    this.Left += e.X - _mouseOffset.X;
                    this.Top += e.Y - _mouseOffset.Y;
                }
            };
            this.MouseUp += async (sender, e) =>
            {
                if (_mouseMoved)
                {
                    _isDragging = false;
                    _mouseMoved = false;
                    bool cardPlayed = await CardReleased.Invoke();
                    if (cardPlayed)
                    {

                    }
                    else
                    {
                        this.Location = _locationBeforeDragging;
                    }
                }
            };
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Vars for paint
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (Card.IsOpen)
            {
                if (Card is LegendCard legendCard)
                {
                    DrawLegendCard(g, legendCard);
                }
                else if (Card is StageCard stageCard)
                {
                    DrawStageCard(g, stageCard);
                }
                else
                {
                    DrawCard(g);
                }
            }
            else
            {
                DrawCardBackSide(g);
            }
            DrawCardBorder(g);

            // Local Methods
            void DrawLegendCard(Graphics g, LegendCard legendCard)
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
                g.SetClip(roundedImagePath);

                // Draw the image with rounded corners
                g.DrawImage(legendCard.Image, new Rectangle(x, y, newWidth, newHeight));

                // Reset the clipping region to its default state
                g.ResetClip();

                // Draw text elements
                Brush textBrush = new SolidBrush(Card.TextColor);
                g.DrawString(legendCard.Name, Font, textBrush, new PointF(5, 5));
                g.DrawString(legendCard.Cost.ToString(), Font, textBrush, new PointF(Width - 20, Height - 25));
                g.DrawString($"HP {legendCard.CurrentHP}/{legendCard.BaseHealth}", Font, textBrush, new PointF(5, Height - 71));
                g.DrawString($"Att {legendCard.Power}", Font, textBrush, new PointF(5, Height - 48));
            }
            void DrawStageCard(Graphics g, StageCard stageCard)
            {
                Brush brush = new SolidBrush(stageCard.CardColor);
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
                g.SetClip(roundedImagePath);

                // Draw the image with rounded corners
                g.DrawImage(stageCard.Image, new Rectangle(x, y, newWidth, newHeight));

                // Reset the clipping region to its default state
                g.ResetClip();

                // Draw text elements
                Brush textBrush = new SolidBrush(Card.TextColor);
                g.DrawString(stageCard.Name, Font, textBrush, new PointF(5, 5));
                g.DrawString(stageCard.Cost.ToString(), Font, textBrush, new PointF(Width - 20, Height - 25));
            }
            void DrawCard(Graphics g)
            {
                Brush cardBrush = new SolidBrush(Card.CardColor);
                g.FillRectangle(cardBrush, 0, 0, Width, Height);
                g.DrawImage(Card.Image, new Rectangle(10, 30, Width - 20, Height - 60));
                Brush textBrush = new SolidBrush(Card.TextColor);
                g.DrawString(Card.Name, Font, textBrush, new PointF(5, 5));
                g.DrawString(Card.Cost.ToString(), Font, textBrush, new PointF(Width - 20, Height - 25));
            }
            void DrawCardBackSide(Graphics g)
            {
                g.FillRectangle(Brushes.LightBlue, 0, 0, Width, Height);
                g.DrawImage(_backSideImage, new Rectangle(10, 30, Width - 20, Height - 60));
            }
            void DrawCardBorder(Graphics g)
            {
                int borderThickness = 3;
                g.DrawRectangle(new Pen(Color.Black, borderThickness), 0, 0, Width - 2, Height - 2);
            }
        }
        public void EnableDragging(bool enable)
        {
            _canDrag = enable;
        }
        public void FlipCard()
        {
            Card.IsOpen = !Card.IsOpen;
            Invalidate();
        }
        void OnCardClicked()
        {
            if (!_game.SomeoneIsAttacking)
            {
                _isDragging = false;
                if (!_mouseMoved)
                {
                    if (Card is LegendCard legendCard)
                    {
                        Form parentForm = this.FindForm();
                        RenderLegendCard(parentForm);
                        RenderWeaponCards(legendCard, parentForm);
                    }
                }
            }

            // Local Methods
            void RenderLegendCard(Form parentForm)
            {
                FRM_PlayingField frm = (FRM_PlayingField)parentForm;
                // Init Card
                DetailedCardControl legendCardControl = new DetailedCardControl(_game, this.Card, Owner, Players, this, UI_ArrangeCardsInPlayingField)
                {
                    Size = new Size(CARD_WIDTH * 3, CARD_HEIGHT * 3), // 3x size
                    Location = new Point(parentForm.ClientSize.Width - CARD_WIDTH * 3 - 20, 20)
                };
                legendCardControl.UI_UpdatePlayerInformation += frm.UpdatePlayerInformation;
                // UI
                parentForm.Controls.Add(legendCardControl);
                legendCardControl.BringToFront();
            }
            void RenderWeaponCards(LegendCard legendCard, Form parentForm)
            {
                List<Card> stackedCards = legendCard.StackedCards;

                int spacing = 20;                      // Spacing between cards
                int cardWidth = (int)(CARD_WIDTH * 1.5);
                int cardHeight = (int)(CARD_HEIGHT * 1.5);

                // Start from the right edge of the form
                int startX = parentForm.ClientSize.Width - cardWidth - spacing;
                int startY = parentForm.ClientSize.Height - cardHeight - spacing; // 20 px from bottom

                foreach (Card weaponCard in stackedCards)
                {
                    DetailedCardControl weaponCardControl = new DetailedCardControl(_game, weaponCard, Owner, Players, this, UI_ArrangeCardsInPlayingField)
                    {
                        Size = new Size(cardWidth, cardHeight),
                        Location = new Point(startX, startY)
                    };

                    parentForm.Controls.Add(weaponCardControl);
                    weaponCardControl.BringToFront();

                    // Move to the left for the next card
                    startX -= cardWidth / 2 + spacing;
                }
            }
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            CardClicked?.Invoke(this); // Trigger the event and pass THIS CardControl
        }
        public void EnemyIsAttacking(Attack attack, LegendCard enemyLegend, DetailedCardControl? detailedCardControl = null)
        {
            _enemyAttack = attack;
            _enemyLegend = enemyLegend;
            _enemyDetailedCardControl = detailedCardControl;
        }

        public void CheckIfDead()
        {
            LegendCard legend = (LegendCard)this.Card;

            // if legend is dead remove from playing field and rearrange cards
            FRM_PlayingField parentForm = (FRM_PlayingField)this.FindForm();
            if (legend.CurrentHP <= 0 && parentForm != null) // if parentform is null it has already been removed
            {
                // move stacked cards to discard pile
                foreach (Card card in legend.StackedCards.ToList())
                {
                    legend.StackedCards.Remove(card);
                    this.Owner.DiscardPile.Add(card);
                }
                // Delete stacked Cards visually
                foreach (CardControl cardControl in this.CardsControls.ToList())
                {
                    parentForm.Controls.Remove(cardControl);
                    cardControl.Invalidate();
                    cardControl.Update();
                    // add to discard pile visually
                }

                // move legend card to discard pile
                this.Owner.PlayingField.Remove(Card);
                this.Owner.DiscardPile.Add(Card);

                // delete legend from playing field
                ZoneControl myZone = parentForm.GetMyZone(ZoneTypes.PlayingField, Owner);
                parentForm.RemoveCardControl(this, myZone);

                // Rearrange playing field
                if (Owner.PlayingField.Count > 0)
                {
                    UI_ArrangeCardsInPlayingField(Owner);
                }

                this.Dispose();
            }
        }
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
