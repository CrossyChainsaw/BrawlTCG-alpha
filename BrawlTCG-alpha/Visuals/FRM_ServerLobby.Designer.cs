namespace BrawlTCG_alpha.Visuals
{
    partial class FRM_ServerLobby
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
            GB_Lobby = new GroupBox();
            BTN_Join_Lobby_2 = new Button();
            TB_Deck = new TextBox();
            label2 = new Label();
            label1 = new Label();
            TB_Username = new TextBox();
            GB_Lobby.SuspendLayout();
            SuspendLayout();
            // 
            // GB_Lobby
            // 
            GB_Lobby.Controls.Add(BTN_Join_Lobby_2);
            GB_Lobby.Controls.Add(TB_Deck);
            GB_Lobby.Controls.Add(label2);
            GB_Lobby.Controls.Add(label1);
            GB_Lobby.Controls.Add(TB_Username);
            GB_Lobby.Location = new Point(12, 12);
            GB_Lobby.Name = "GB_Lobby";
            GB_Lobby.Size = new Size(231, 141);
            GB_Lobby.TabIndex = 0;
            GB_Lobby.TabStop = false;
            GB_Lobby.Text = "Join Lobby";
            // 
            // BTN_Join_Lobby_2
            // 
            BTN_Join_Lobby_2.Location = new Point(6, 99);
            BTN_Join_Lobby_2.Name = "BTN_Join_Lobby_2";
            BTN_Join_Lobby_2.Size = new Size(206, 29);
            BTN_Join_Lobby_2.TabIndex = 4;
            BTN_Join_Lobby_2.Text = "Join Lobby";
            BTN_Join_Lobby_2.UseVisualStyleBackColor = true;
            BTN_Join_Lobby_2.Click += BTN_Join_Lobby_2_Click;
            // 
            // TB_Deck
            // 
            TB_Deck.Location = new Point(87, 66);
            TB_Deck.Name = "TB_Deck";
            TB_Deck.Size = new Size(125, 27);
            TB_Deck.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 69);
            label2.Name = "label2";
            label2.Size = new Size(42, 20);
            label2.TabIndex = 2;
            label2.Text = "Deck";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 33);
            label1.Name = "label1";
            label1.Size = new Size(75, 20);
            label1.TabIndex = 1;
            label1.Text = "Username";
            // 
            // TB_Username
            // 
            TB_Username.Location = new Point(87, 33);
            TB_Username.Name = "TB_Username";
            TB_Username.Size = new Size(125, 27);
            TB_Username.TabIndex = 0;
            // 
            // FRM_ServerLobby
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.hellboy;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(GB_Lobby);
            Name = "FRM_ServerLobby";
            Text = "FRM_ServerLobby";
            GB_Lobby.ResumeLayout(false);
            GB_Lobby.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox GB_Lobby;
        private TextBox TB_Deck;
        private Label label2;
        private Label label1;
        private TextBox TB_Username;
        private Button BTN_Join_Lobby_2;
    }
}