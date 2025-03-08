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
    public class CardControl : Control
    {
        // Constants
        const int CARD_WIDTH = 150;
        const int CARD_HEIGHT = 200;

        // Fields
        private Image _backSideImage = Properties.Resources.BrawlLogo;
        private bool _isDragging = false;
        private bool _mouseMoved = false;
        private bool _canDrag = true;
        private Point _mouseOffset;
        private Point _locationBeforeDragging;
        private Game _game;

        // Properties
        public Card Card { get; private set; }
        public List<CardControl> CardsControls { get; internal set; }
        public Player? Owner { get; private set; }
        public List<Player> Players { get; internal set; }

        // Events
        public event Func<Task<bool>>? CardReleased;
        public event Action<CardControl> CardClicked; // Event to notify when this card is clicked
        public event Action<Player> UI_ArrangeCardsInPlayingField;
        public event Action<Player, CardControl> UI_AddCardToDiscardPile;
        public event Action<Player> UI_UpdatePlayerInformation;


        // Methods
        public CardControl(Game game, Card card, Action<Player> UI_arrangeCardsFunction, bool isOpen = false, Player? owner = null, List<Player> players = null)
        {
            // Initialize properties
            Card = card;
            this.Card.IsOpen = isOpen;
            Owner = owner;
            CardsControls = new List<CardControl>();
            Players = players;
            _game = game;

            // Card Appearence
            Size = new Size(CARD_WIDTH, CARD_HEIGHT);
            Font = new Font("Arial", 12, FontStyle.Bold);

            // Delegates
            UI_ArrangeCardsInPlayingField += UI_arrangeCardsFunction;

            // Events
            this.Click += (sender, e) => { OnCardClicked(); };

            // Dragging
            this.MouseDown += (sender, e) =>
            {
                if (_canDrag && !Card.IsDiscarded && Owner == _game.ActivePlayer) // i.e. can drag && card is not in discard pile && it is your own card
                {
                    StartDragging(e);
                }
            };
            this.MouseMove += (sender, e) =>
            {
                if (_isDragging)
                {
                    DragCard(e);
                }
            };
            this.MouseUp += async (sender, e) =>
            {
                if (_mouseMoved)
                {
                    await EndDragging();
                }
            };
        }


        // Override
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            CardClicked?.Invoke(this); // Trigger the event and pass THIS CardControl
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
                    PaintLegendCard(g, legendCard);
                }
                else if (Card is StageCard stageCard)
                {
                    PaintStageCard(g, stageCard);
                }
                else
                {
                    PaintAnyOtherCard(g);
                }
            }
            else
            {
                PaintCardBackSide(g);
            }
            PaintCardBorder(g);

        }



        // Paint
        void PaintLegendCard(Graphics g, LegendCard legendCard)
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

        void PaintStageCard(Graphics g, StageCard stageCard)
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

        void PaintAnyOtherCard(Graphics g)
        {
            Brush cardBrush = new SolidBrush(Card.CardColor);
            g.FillRectangle(cardBrush, 0, 0, Width, Height);
            g.DrawImage(Card.Image, new Rectangle(10, 30, Width - 20, Height - 60));
            Brush textBrush = new SolidBrush(Card.TextColor);
            g.DrawString(Card.Name, Font, textBrush, new PointF(5, 5));
            g.DrawString(Card.Cost.ToString(), Font, textBrush, new PointF(Width - 20, Height - 25));
        }

        void PaintCardBackSide(Graphics g)
        {
            g.FillRectangle(Brushes.LightBlue, 0, 0, Width, Height);
            g.DrawImage(_backSideImage, new Rectangle(10, 30, Width - 20, Height - 60));
        }

        void PaintCardBorder(Graphics g)
        {
            int borderThickness = 3;
            g.DrawRectangle(new Pen(Color.Black, borderThickness), 0, 0, Width - 2, Height - 2);
        }



        // Dragging Logic
        void StartDragging(MouseEventArgs e)
        {
            _isDragging = true;
            _mouseOffset = e.Location;  // Remember where the mouse was clicked relative to the card
            _locationBeforeDragging = this.Location;
        }

        void DragCard(MouseEventArgs e)
        {
            _mouseMoved = true;
            // Calculate the new location based on the mouse movement
            this.Left += e.X - _mouseOffset.X;
            this.Top += e.Y - _mouseOffset.Y;
        }

        async Task EndDragging()
        {
            _isDragging = false;
            _mouseMoved = false;
            bool cardPlayed = await CardReleased?.Invoke();

            if (cardPlayed)
            {
                // Handle card played logic (if needed)
            }
            else
            {
                // Reset card to original position if not played
                this.Location = _locationBeforeDragging;
            }
        }

        public void SetCanDrag(bool enable)
        {
            _canDrag = enable;
        }



        // Render When Clicked
        void OnCardClicked()
        {
            if (!_game.GetSomeoneIsAttacking())
            {
                _isDragging = false;
                if (!_mouseMoved)
                {
                    Form parentForm = this.FindForm();
                    if (Card is LegendCard legendCard)
                    {
                        DetailedCardControl dcc = RenderLegendCard(parentForm);
                        RenderWeaponCards(legendCard, parentForm, dcc);
                    }
                    else
                    {
                        RenderCard(parentForm);
                    }
                }
            }

            // Local Methods
            DetailedCardControl RenderCard(Form parentForm)
            {
                FRM_Game frm = (FRM_Game)parentForm;
                // Init Card
                DetailedCardControl dcc = new DetailedCardControl(_game, this.Card, Owner, Players, this, UI_ArrangeCardsInPlayingField)
                {
                    Size = new Size(CARD_WIDTH * 3, CARD_HEIGHT * 3), // 3x size
                    Location = new Point(parentForm.ClientSize.Width - CARD_WIDTH * 3 - 20, 20)
                };
                // UI
                parentForm.Controls.Add(dcc);
                dcc.BringToFront();

                return dcc;
            }
            DetailedCardControl RenderLegendCard(Form parentForm)
            {
                FRM_Game frm = (FRM_Game)parentForm;
                // Init Card
                DetailedCardControl legendCardControl = new DetailedCardControl(_game, this.Card, Owner, Players, this, UI_ArrangeCardsInPlayingField)
                {
                    Size = new Size(CARD_WIDTH * 3, CARD_HEIGHT * 3), // 3x size
                    Location = new Point(parentForm.ClientSize.Width - CARD_WIDTH * 3 - 20, 20)
                };
                legendCardControl.UI_UpdatePlayerInformation += frm.UI.UpdatePlayerInformation; // this is a bad practise no?
                legendCardControl.NETWORK_SendMessage += frm.Network.SendMessageToPeer;
                // UI
                parentForm.Controls.Add(legendCardControl);
                legendCardControl.BringToFront();

                return legendCardControl;
            }
            void RenderWeaponCards(LegendCard legendCard, Form parentForm, DetailedCardControl dcc)
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
                        Location = new Point(startX, startY),
                    };
                    // Add to Memory
                    dcc.WeaponCardControls.Add(weaponCardControl);
                    // Add to UI
                    parentForm.Controls.Add(weaponCardControl);
                    weaponCardControl.BringToFront();

                    // Move to the left for the next card
                    startX -= cardWidth / 2 + spacing;
                }
            }
        }



        // Combat
        public void CheckIfDead()
        {
            LegendCard legend = (LegendCard)this.Card;

            // if legend is dead remove from playing field and rearrange cards
            FRM_Game parentForm = (FRM_Game)this.FindForm();
            if (legend.CurrentHP <= 0 && parentForm != null) // if parentform is null it has already been removed
            {
                // correct hp
                legend.CurrentHP = 0;

                // Remove stacked cards - (logically the stacked card is still here but the control is removed)
                int n = legend.StackedCards.Count;
                for (int i = 0; i < n; i++)
                {
                    // Remove card logically
                    Card card = legend.StackedCards[0];
                    legend.StackedCards.Remove(card);
                    _game.AddCardToDiscardPile(Owner, card);

                    // Delete card visually
                    CardControl cardControl = this.CardsControls[0];
                    this.CardsControls.Remove(cardControl);
                    UI_AddCardToDiscardPile(Owner, cardControl);
                }

                // disable attack buttons by saying its not on the playing field anymore
                legend.OnPlayingField = false;

                // move legend card to discard pile logically
                this.Owner.PlayingField.Remove(Card);
                _game.AddCardToDiscardPile(Owner, Card);
                // visually
                UI_AddCardToDiscardPile(Owner, this);

                // Rearrange playing field
                if (Owner.PlayingField.Count > 0)
                {
                    UI_ArrangeCardsInPlayingField(Owner);
                } 

                this.Dispose();
            }
        }

        public void FlipCard()
        {
            Card.IsOpen = !Card.IsOpen;
            Invalidate();
        }

        // THESE FUNCTIONS ARE FROM DCC AND AREN'T SUPOSED TO BE USED, EXCEPT FOR THE PLAYER THAT LISTENED TO A MESSAGE, THEY CAN TRIGGER THESE.
        public void AttackThePlayer(CardControl legendCC, Player otherPlayer, Attack attack)
        {
            LegendCard legendCard = (LegendCard)legendCC.Card;

            // Attack
            attack.Effect.Invoke(legendCard, otherPlayer, attack, _game.ActivePlayer, _game); // send attack name? // attacking legend card index
            UI_UpdatePlayerInformation(otherPlayer);

            // Notify
            MessageBox.Show($"{otherPlayer.Name} just took damage");

            // Check if dead
            if (otherPlayer.Health <= 0)
                MessageBox.Show($"{otherPlayer.Name} has been defeated");
            legendCC.CheckIfDead();
        }
        public void AttackLegendCard(CardControl legendCC, CardControl enemyCC)
        {
            LegendCard legendCard = (LegendCard)legendCC.Card;
            LegendCard targetLegend = (LegendCard)enemyCC.Card;

            // Apply the Damage
            _game.GetSelectedAttack().Effect.Invoke(legendCard, targetLegend, _game.GetSelectedAttack(), _game.ActivePlayer, _game);
            
            // Update cc
            enemyCC.Invalidate();
            legendCC.Invalidate();

            // CHECK IF DEAD
            enemyCC.CheckIfDead();
            legendCC.CheckIfDead();

            // stop attacking
            _game.StopAttack();
        }
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
