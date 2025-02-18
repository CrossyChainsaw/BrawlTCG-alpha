namespace BrawlTCG_alpha.Visuals
{
    namespace BrawlTCG_alpha.Visuals
    {
        partial class FormPlayerNames
        {
            private System.ComponentModel.IContainer components = null;
            private TextBox txtPlayer1Name;
            private TextBox txtPlayer2Name;
            private Label lblPlayer1Name;
            private Label lblPlayer2Name;
            private Button btnSubmit;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                this.txtPlayer1Name = new TextBox();
                this.txtPlayer2Name = new TextBox();
                this.lblPlayer1Name = new Label();
                this.lblPlayer2Name = new Label();
                this.btnSubmit = new Button();

                // 
                // txtPlayer1Name
                // 
                this.txtPlayer1Name.Location = new System.Drawing.Point(150, 30);
                this.txtPlayer1Name.Name = "txtPlayer1Name";
                this.txtPlayer1Name.Size = new System.Drawing.Size(200, 20);

                // 
                // txtPlayer2Name
                // 
                this.txtPlayer2Name.Location = new System.Drawing.Point(150, 80);
                this.txtPlayer2Name.Name = "txtPlayer2Name";
                this.txtPlayer2Name.Size = new System.Drawing.Size(200, 20);

                // 
                // lblPlayer1Name
                // 
                this.lblPlayer1Name.AutoSize = true;
                this.lblPlayer1Name.Location = new System.Drawing.Point(50, 30);
                this.lblPlayer1Name.Name = "lblPlayer1Name";
                this.lblPlayer1Name.Size = new System.Drawing.Size(94, 13);
                this.lblPlayer1Name.Text = "Enter Player 1 Name:";

                // 
                // lblPlayer2Name
                // 
                this.lblPlayer2Name.AutoSize = true;
                this.lblPlayer2Name.Location = new System.Drawing.Point(50, 80);
                this.lblPlayer2Name.Name = "lblPlayer2Name";
                this.lblPlayer2Name.Size = new System.Drawing.Size(94, 13);
                this.lblPlayer2Name.Text = "Enter Player 2 Name:";

                // 
                // btnSubmit
                // 
                this.btnSubmit.Location = new System.Drawing.Point(150, 130);
                this.btnSubmit.Name = "btnSubmit";
                this.btnSubmit.Size = new System.Drawing.Size(75, 30);
                this.btnSubmit.Text = "Submit";
                this.btnSubmit.Click += new EventHandler(this.btnSubmit_Click);

                // 
                // FormPlayerNames
                // 
                this.ClientSize = new System.Drawing.Size(400, 200);
                this.Controls.Add(this.txtPlayer1Name);
                this.Controls.Add(this.txtPlayer2Name);
                this.Controls.Add(this.lblPlayer1Name);
                this.Controls.Add(this.lblPlayer2Name);
                this.Controls.Add(this.btnSubmit);
                this.Name = "FormPlayerNames";
                this.Text = "Enter Player Names";
            }
        }
    }

}