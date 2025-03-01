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
            BTN_EditDeck = new Button();
            TB_Name = new TextBox();
            label1 = new Label();
            BTN_P2P = new Button();
            label2 = new Label();
            TB_Deck = new TextBox();
            LBL_Status = new Label();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // BTN_EditDeck
            // 
            BTN_EditDeck.Location = new Point(6, 26);
            BTN_EditDeck.Name = "BTN_EditDeck";
            BTN_EditDeck.Size = new Size(181, 29);
            BTN_EditDeck.TabIndex = 2;
            BTN_EditDeck.Text = "Edit Deck";
            BTN_EditDeck.UseVisualStyleBackColor = true;
            BTN_EditDeck.Click += BTN_EditDeck_OnClick;
            // 
            // TB_Name
            // 
            TB_Name.Location = new Point(61, 24);
            TB_Name.Name = "TB_Name";
            TB_Name.Size = new Size(125, 27);
            TB_Name.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 28);
            label1.Name = "label1";
            label1.Size = new Size(49, 20);
            label1.TabIndex = 3;
            label1.Text = "Name";
            // 
            // BTN_P2P
            // 
            BTN_P2P.Location = new Point(6, 90);
            BTN_P2P.Name = "BTN_P2P";
            BTN_P2P.Size = new Size(181, 29);
            BTN_P2P.TabIndex = 6;
            BTN_P2P.Text = "Multiplayer";
            BTN_P2P.UseVisualStyleBackColor = true;
            BTN_P2P.Click += BTN_P2P_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 60);
            label2.Name = "label2";
            label2.Size = new Size(42, 20);
            label2.TabIndex = 5;
            label2.Text = "Deck";
            // 
            // TB_Deck
            // 
            TB_Deck.Location = new Point(61, 57);
            TB_Deck.Name = "TB_Deck";
            TB_Deck.Size = new Size(125, 27);
            TB_Deck.TabIndex = 1;
            TB_Deck.Text = "deckPlayer1";
            TB_Deck.TextChanged += TB_Deck_TextChanged;
            // 
            // LBL_Status
            // 
            LBL_Status.AutoSize = true;
            LBL_Status.Location = new Point(6, 122);
            LBL_Status.Name = "LBL_Status";
            LBL_Status.Size = new Size(156, 20);
            LBL_Status.TabIndex = 7;
            LBL_Status.Text = "Status: Not Connected";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(BTN_P2P);
            groupBox1.Controls.Add(TB_Name);
            groupBox1.Controls.Add(LBL_Status);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(TB_Deck);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(12, 86);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(193, 150);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Online";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(BTN_EditDeck);
            groupBox2.Location = new Point(12, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(193, 68);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Local";
            groupBox2.Enter += groupBox2_Enter;
            // 
            // FRM_Menu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(542, 385);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "FRM_Menu";
            Text = "FRM_Menu";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button BTN_EditDeck;
        private TextBox TB_Name;
        private Label label1;
        private Button BTN_P2P;
        private Label label2;
        private TextBox TB_Deck;
        private Label LBL_Status;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
    }
}