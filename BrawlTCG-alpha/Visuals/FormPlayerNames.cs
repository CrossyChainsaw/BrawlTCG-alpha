using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrawlTCG_alpha.Visuals
{
    using System;
    using System.Windows.Forms;

    namespace BrawlTCG_alpha.Visuals
    {
        public partial class FormPlayerNames : Form
        {
            public string Player1Name { get; private set; }
            public string Player2Name { get; private set; }

            public FormPlayerNames()
            {
                InitializeComponent();
            }

            private void btnSubmit_Click(object sender, EventArgs e)
            {
                // Get names from TextBox inputs
                Player1Name = txtPlayer1Name.Text;
                Player2Name = txtPlayer2Name.Text;

                // Validate the input (you can add more validation if needed)
                if (string.IsNullOrEmpty(Player1Name) || string.IsNullOrEmpty(Player2Name))
                {
                    MessageBox.Show("Please enter both Player 1 and Player 2 names.");
                    return;
                }

                this.DialogResult = DialogResult.OK; // Indicate that the form was submitted successfully
                this.Close(); // Close the form after submission
            }
        }
    }

}
