using BrawlTCG_alpha.Logic;
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
    public partial class FRM_SetupGame : Form
    {
        public FRM_SetupGame()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Any sort of relation to logic
            Player player1 = new Player("John", CardCatalogue.CloneList(CardCatalogue.TestDeck));
            Player player2 = new Player("Jane", CardCatalogue.CloneList(CardCatalogue.DeathCapDeck));

            FRM_Game frm_game = new FRM_Game(player1, player2);
            frm_game.Show();
        }
    }
}
