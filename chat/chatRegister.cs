using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using chat.models;
using EI.SI;
using static chat.FormChatLogin;

namespace chat
{


    public partial class formChatRegister : Form
    {
        private const int Port = 10000;
        public static NetworkStream netStream;
        private string privKey;
        private string pubKey;

        TcpClient tcpClient;
        ProtocolSI protocolSI;
        public formChatRegister(TcpClient tcpClient, NetworkStream netStream, string privKey, string pubKey)
        {
            this.tcpClient = tcpClient;
            this.privKey = privKey;
            this.pubKey = pubKey;
            InitializeComponent();
            protocolSI = new ProtocolSI();
           
            ReceiveNetworkStream(netStream);
            
        }

        private void chatRegister_Load(object sender, EventArgs e)
        {
            labelErrorShower.Text = "";
        }

        private void textBoxUserNameRegister_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxUserNameRegister.Text == "Username")
            {
                textBoxUserNameRegister.Text = "";
                registerButtonControl();
            }
        }
        private void registerButtonControl()
        {
            if (textBoxUserNameRegister.Text != "Username" && textBoxEmailRegister.Text != "Email" && textBoxPasswordRegister.Text != "Password" && textBoxConfirmPasswordRegister.Text != "Confirm Password")
            {
                buttonChatRegister.Enabled = true;
            }
            else
            {
                buttonChatRegister.Enabled = false;
            }
        }

        private void textBoxUserNameRegister_Leave(object sender, EventArgs e)
        {
            if (textBoxUserNameRegister.Text == "") {
                textBoxUserNameRegister.Text = "Username";
            }
        }

        private void textBoxEmailRegister_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxEmailRegister.Text == "Email")
            {
                textBoxEmailRegister.Text = "";
                registerButtonControl();
            }
        }

        private void textBoxEmailRegister_Leave(object sender, EventArgs e)
        {
            if (textBoxEmailRegister.Text == "")
            {
                textBoxEmailRegister.Text = "Email";
            }
        }

        private void textBoxPasswordRegistor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxPasswordRegister.Text == "Password")
            {
                textBoxPasswordRegister.Text = "";
                registerButtonControl();
            }
        }

        private void textBoxPasswordRegistor_Leave(object sender, EventArgs e)
        {
            if (textBoxPasswordRegister.Text == "")
            {
                textBoxPasswordRegister.Text = "Password";
            }
        }

        private void textBoxConfirmPasswordRegistor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxConfirmPasswordRegister.Text == "Confirm Password")
            {
                textBoxConfirmPasswordRegister.Text = "";
                registerButtonControl();
            }
        }

        private void textBoxConfirmPasswordRegistor_Leave(object sender, EventArgs e)
        {
            if (textBoxConfirmPasswordRegister.Text == "")
            {
                textBoxConfirmPasswordRegister.Text = "Confirm Password";
            }
        }
        

        private void changeForm(string response)
        {
            if (response == "OK")
            {
                MessageBox.Show("Register Sucessful");
                FormChatLogin LoginForm = new FormChatLogin(tcpClient, pubKey, privKey);
                this.Close();
                LoginForm.Show();
            }
            else
            {
                MessageBox.Show("Register failed. Please try again.");
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormChatLogin LoginForm = new FormChatLogin(tcpClient, pubKey, privKey);
            LoginForm.ShowDialog();
            this.Close();
        }

        private void buttonChatRegister_Click_1(object sender, EventArgs e)
        {
            string username = textBoxUserNameRegister.Text;
            string password = textBoxPasswordRegister.Text;
            string passwordConfirmation = textBoxConfirmPasswordRegister.Text;
            string email = textBoxEmailRegister.Text;
            string response;
            int size;


            if (string.IsNullOrEmpty(username)
                || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(passwordConfirmation)
                || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("All fields are required"); 
                return;
            }

            if (password != passwordConfirmation)
            {
                MessageBox.Show("Password mismatch");
                return;
            }

            if (password.Count() < 8)
            {
                MessageBox.Show(" Password must be over 8 characters");
                return;
            }

            try
            {
                MailAddress adress = new MailAddress(email);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Email");
                return;
            }

            string msg = "Register-" + username + "-" + password + "-" + email;
            string cryptedText = CryptFunctions.encryptText(msg, pubKey);
            byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_3, cryptedText);
            netStream.Write(packet, 0, packet.Length);

            size = netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size);
            string plainText = CryptFunctions.decryptText(response, pubKey);

            changeForm(plainText);
        }

        public static void ReceiveNetworkStream(NetworkStream Stream)
        {
            netStream = Stream;
        }
    }
}
