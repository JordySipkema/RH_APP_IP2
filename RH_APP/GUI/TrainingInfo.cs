using System;
using System.Windows.Forms;

namespace RH_APP.GUI
{
    public partial class TrainingInfo : Form
    {
        public TrainingInfo()
        {
            InitializeComponent();
        }

        private void TrainingInfo_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var mainScreen = new TrainingScreen(false) {Text = " Remote Healthcare - Client Edition"};
            mainScreen.ShowDialog();
        }
    }
}
