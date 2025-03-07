namespace BrawlTCG_alpha.Visuals
{
    partial class FRM_Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_Menu));
            BTN_EditDeck = new Button();
            TB_Name = new TextBox();
            BTN_P2P = new Button();
            TB_Deck = new TextBox();
            LBL_Status = new Label();
            label1 = new Label();
            label2 = new Label();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // BTN_EditDeck
            // 
            BTN_EditDeck.BackColor = Color.MediumPurple;
            BTN_EditDeck.Font = new Font("Program OT Bold", 72F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BTN_EditDeck.Location = new Point(12, 12);
            BTN_EditDeck.Name = "BTN_EditDeck";
            BTN_EditDeck.Size = new Size(702, 155);
            BTN_EditDeck.TabIndex = 2;
            BTN_EditDeck.Text = "EDIT DECK";
            BTN_EditDeck.UseVisualStyleBackColor = false;
            BTN_EditDeck.Click += BTN_EditDeck_OnClick;
            // 
            // TB_Name
            // 
            TB_Name.BackColor = Color.White;
            TB_Name.Location = new Point(5, 26);
            TB_Name.Name = "TB_Name";
            TB_Name.PlaceholderText = "Name";
            TB_Name.Size = new Size(180, 27);
            TB_Name.TabIndex = 0;
            // 
            // BTN_P2P
            // 
            BTN_P2P.Location = new Point(5, 92);
            BTN_P2P.Name = "BTN_P2P";
            BTN_P2P.Size = new Size(181, 29);
            BTN_P2P.TabIndex = 6;
            BTN_P2P.Text = "Multiplayer";
            BTN_P2P.UseVisualStyleBackColor = true;
            BTN_P2P.Click += BTN_P2P_Click;
            // 
            // TB_Deck
            // 
            TB_Deck.BackColor = Color.White;
            TB_Deck.Location = new Point(5, 59);
            TB_Deck.Name = "TB_Deck";
            TB_Deck.Size = new Size(180, 27);
            TB_Deck.TabIndex = 1;
            TB_Deck.Text = "deckPlayer1";
            TB_Deck.TextChanged += TB_Deck_TextChanged;
            // 
            // LBL_Status
            // 
            LBL_Status.Location = new Point(5, 124);
            LBL_Status.Name = "LBL_Status";
            LBL_Status.Size = new Size(180, 23);
            LBL_Status.TabIndex = 7;
            LBL_Status.Text = "Status: Not Connected";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(464, 369);
            label1.Name = "label1";
            label1.Size = new Size(250, 20);
            label1.TabIndex = 8;
            label1.Text = "Guys don't hate this will change trust";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Location = new Point(6, 0);
            label2.Name = "label2";
            label2.Size = new Size(180, 23);
            label2.TabIndex = 9;
            label2.Text = "Play Online";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(LBL_Status);
            groupBox1.Controls.Add(TB_Deck);
            groupBox1.Controls.Add(TB_Name);
            groupBox1.Controls.Add(BTN_P2P);
            groupBox1.Location = new Point(13, 188);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(701, 155);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            // 
            // FRM_Menu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 192, 255);
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(726, 410);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Controls.Add(BTN_EditDeck);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FRM_Menu";
            Text = "Menu";
            TransparencyKey = Color.RosyBrown;
            WindowState = FormWindowState.Maximized;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BTN_EditDeck;
        private TextBox TB_Name;
        private Button BTN_P2P;
        private TextBox TB_Deck;
        private Label LBL_Status;
        private Label label1;
        private Label label2;
        private GroupBox groupBox1;
    }
}