#pragma warning disable CS8602 // Dereference of a possibly null reference.
using BrawlTCG_alpha.Logic;
using BrawlTCG_alpha.Logic.Cards;
using BrawlTCG_alpha.Logic.Managers;
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
        private bool _isDragging = false;
        private bool _mouseMoved = false;
        private bool _canDrag = true;
        private Point _mouseOffset;
        private Point _locationBeforeDragging;
        private Game _game;
        private PaintCardManager _paintCardManager;


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
            _paintCardManager = new PaintCardManager();

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
                if (_canDrag && !Card.IsDiscarded && Owner == _game.ActivePlayer && Card.IsOpen) // i.e. can drag && card is not in discard pile && it is your own card
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
            _paintCardManager.PaintCardCC(e, Card);
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
            StopDragging();
            if (!_game.GetSomeoneIsAttacking() && Card.IsOpen)
            {
                if (!_mouseMoved)
                {
                    RemoveActiveDCC();

                    Form parentForm = this.FindForm();
                    if (Card is LegendCard legendCard)
                    {
                        // Visually
                        DetailedCardControl dcc = RenderLegendCard(parentForm);
                        RenderWeaponCards(legendCard, parentForm, dcc);
                        // Logically
                        _game.UiManager.SetDCC(dcc);
                    }
                    else
                    {
                        // Visually
                        DetailedCardControl dcc = RenderCard(parentForm);
                        // Logically
                        _game.UiManager.SetDCC(dcc);
                    }
                }
            }

            // Local Methods
            void StopDragging()
            {
                _isDragging = false;
            }
            void RemoveActiveDCC()
            {
                // if there is an active dcc, remove it
                if (_game.UiManager.ActiveDCC != null)
                {
                    _game.UiManager.ActiveDCC.OnDetailedCardClicked();
                }
            }
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
                        Scale = 1.5f
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



        // THESE FUNCTIONS ARE ONLY ALLOWED TO BE USED FROM NETWORK MANAGER
        public void AttackThePlayer(CardControl legendCC, Player otherPlayer, Attack attack)
        {
            LegendCard legend = (LegendCard)legendCC.Card;

            // Attack
            attack.Effect.Invoke(legend, otherPlayer, attack, _game.ActivePlayer, _game); // send attack name? // attacking legend card index
            UI_UpdatePlayerInformation(otherPlayer);
            // Burn Weapons
            legend.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
            legend.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);


            // Notify
            MessageBox.Show($"{otherPlayer.Name} just took damage");

            // Check if dead
            if (otherPlayer.Health <= 0)
                MessageBox.Show($"{otherPlayer.Name} has been defeated");
            legendCC.CheckIfDead();
        }

        public void AttackLegendCard(CardControl legendCC, CardControl enemyCC)
        {
            LegendCard legend = (LegendCard)legendCC.Card;
            LegendCard targetLegend = (LegendCard)enemyCC.Card;

            // Apply the Damage
            Attack attack = _game.GetSelectedAttack();
            attack.Effect.Invoke(legend, targetLegend, _game.GetSelectedAttack(), _game.ActivePlayer, _game);
            // Burn Weapons
            legend.BurnWeapon(attack.WeaponOne, attack.WeaponOneBurnAmount);
            legend.BurnWeapon(attack.WeaponTwo, attack.WeaponTwoBurnAmount);

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
