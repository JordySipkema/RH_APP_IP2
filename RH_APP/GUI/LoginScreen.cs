using System;
using System.Windows.Forms;
using Mallaca;
using Mallaca.Network;
using Mallaca.Network.Packet.Request;
using Mallaca.Network.Packet.Response;
using Mallaca.Network.Packet;

namespace RH_APP.GUI
{
    public partial class LoginScreen : Form
    {
        public LoginScreen()
        {
            InitializeComponent();
            _passwordBox.UseSystemPasswordChar = true;
            TCPController.RunClient();
            TCPController.OnPacketReceived += LoginPacketResponse;
            TCPController.ReceiveTransmissionAsync();
        }

        private void _loginButton_Click(object sender, EventArgs e)
        { 

            var p = new LoginPacket(_usernameBox.Text, Hashing.CreateSHA256(_passwordBox.Text));
            TCPController.Send(p.ToString());
        }

        private void LoginPacketResponse(Packet p)
        {
            var resp = p as LoginResponsePacket;

            if (resp == null)
                return;

            if (this.InvokeRequired)
            {
                this.Invoke((new Action(() => LoginPacketResponse(p))));
                return;
            }


            if (resp.Status == "200")
            {

                RH_APP.Classes.Settings.GetInstance().authToken = resp.AuthToken;
                RH_APP.Classes.Settings.GetInstance().CurrentUser = resp.User;
                if (resp.User.IsSpecialist || resp.User.IsAdministrator)
                {
                    //var mainScreen = new MainScreen(true);
                    TCPController.OnPacketReceived -= LoginPacketResponse;
                    //mainScreen.ShowDialog();

                    this.Hide();
                    MainMenu m = new MainMenu();
                    m.Show();
                }
                else if (resp.User.IsClient)
                {
                        this.Hide();
                        TCPController.OnPacketReceived -= LoginPacketResponse;
                        //var mainScreen = new MainScreen(false);
                    var infoScreen = new TrainingInfo();
                    infoScreen.ShowDialog();

                }
            }
            else
            {
                MessageBox.Show(resp.Description, "Your application has been reviewed");
            }

            

        }

        

        private void _passwordBox_KeyPress(object sender, KeyPressEventArgs e)
        {
           if(e.KeyChar == (Char)Keys.Return)
                _loginButton_Click(this, null);
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {

        }


    }
}
