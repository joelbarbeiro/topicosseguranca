using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using chat;
using EI.SI;

namespace chat
{
    public partial class FormChatLogin : Form
    {

        private const int Port = 10000;
        NetworkStream netStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;

        public FormChatLogin()
        {
            InitializeComponent();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, Port);
            tcpClient = new TcpClient();
            tcpClient.Connect(endPoint);
            netStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();
        }
     


        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string msg ="Login-"+textBoxLoginUsername.Text +"-"+ textBoxLoginPassword.Text;
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
            netStream.Write(packet, 0, packet.Length);
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                changeForm(netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length).ToString());
            }
        }

        private void textBoxLoginUsername_Enter(object sender, EventArgs e)
        {
            textBoxLoginUsername.Text = "";
            loginButtonControl();
        }
        private void textBoxLoginUsername_Leave(object sender, EventArgs e)
        {
            if(textBoxLoginUsername.Text.Length == 0)
            {
                textBoxLoginUsername.Text = "Username:";
            }
            loginButtonControl();
        }
        private void textBoxLoginPassword_Enter(object sender, EventArgs e)
        {
            textBoxLoginPassword.Text = "";
            loginButtonControl();
        }
        private void textBoxLoginPassword_Leave(object sender, EventArgs e)
        {
            if (textBoxLoginPassword.Text.Length == 0)
            {
                textBoxLoginPassword.Text = "Password:";
            }
            loginButtonControl();
        }

        private void loginButtonControl()
        {
            if ((textBoxLoginUsername.Text.Length == 0 || textBoxLoginUsername.Text != "Username:") && (textBoxLoginPassword.Text.Length == 0 || textBoxLoginPassword.Text != "Password:"))
            {
                buttonLogin.Enabled = true;
            }
            else
            {
                buttonLogin.Enabled = false;
            }
        }

        private void changeForm(string response)
        {
            
            //FormChatLogin loginForm = new FormChatLogin();
            if (response == "2")
            {
                FormChat mainForm = new FormChat();
                mainForm.Show();
                this.Hide();
            }
            else
            { 
                MessageBox.Show("Login failed. Please try again.");
            }
        }

        private void buttonChatRegister_Click(object sender, EventArgs e)
        {
            formChatRegister chatRegisterForm = new formChatRegister();
            chatRegisterForm.Show();
            this.Hide();

        }
        private void buttonChatRegister_Click_1(object sender, EventArgs e)
        {
             formChatRegister formchatregister = new formChatRegister();
             formchatregister.Show();
             this.Hide();
        }
    }
}
