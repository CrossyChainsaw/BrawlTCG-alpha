using BrawlTCG_alpha.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlTCG_alpha.Visuals
{
    public enum ZoneTypes
    {
        Deck,
        Hand,
        DiscardPile,
        Stage,
        PlayingField,
        EssenceField,
        PlayerInfo
    }

    public  class ZoneControl : UserControl
    {
        // Fields
        public ZoneTypes ZoneType { get; internal set; }
        public Player Owner;
        public List<CardControl> CardsControls { get; internal set; }
        Color _zoneColor = SystemColors.ControlDarkDark;
        public Label Label;

        // Border properties
        private Color borderColor = Color.Black;
        private int borderThickness = 2;


        // Methods
        public ZoneControl(string name, int width, int height, ZoneTypes zoneType, Player owner)
        {
            this.Size = new Size(width, height);
            this.BackColor = _zoneColor;
            CardsControls = new List<CardControl>();
            ZoneType = zoneType;
            Owner = owner;

            // Initialize the label
            Label = new Label
            {
                Text = name,
                Location = new Point(5, 5),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            this.Controls.Add(Label);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Draw the zone's background (you can customize it)
            g.FillRectangle(new SolidBrush(_zoneColor), 0, 0, Width, Height);

            // Draw the border around the control (outside border)
            using (Pen borderPen = new Pen(borderColor, borderThickness))
            {
                g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }
        }
    }

}
