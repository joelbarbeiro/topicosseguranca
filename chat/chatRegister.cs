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
using System.Threading;
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
        private string serverPubKey;

        TcpClient tcpClient;
        ProtocolSI protocolSI;
        public formChatRegister(TcpClient tcpClient, NetworkStream netStream, string privKey, string pubKey, string serverPubKey)
        {
            this.tcpClient = tcpClient;
            this.privKey = privKey;
            this.pubKey = pubKey;
            this.serverPubKey = serverPubKey;

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
                //FormChatLogin LoginForm = new FormChatLogin(tcpClient, privKey, pubKey, serverPubKey);
                FormChatLogin LoginForm = new FormChatLogin();
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
            //FormChatLogin LoginForm = new FormChatLogin(tcpClient, pubKey, privKey, serverPubKey);
            FormChatLogin LoginForm = new FormChatLogin();
            LoginForm.ShowDialog();
            this.Close();
        }

        private async void buttonChatRegister_Click_1(object sender, EventArgs e)
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

            string plainText = string.Empty;
            byte[] packet = null;


            if (serverPubKey == string.Empty)
            {
                string msg = CryptFunctions.AESEncrypt(pubKey);
                // Envio da public key do cliente
                packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, msg);
                await netStream.WriteAsync(packet, 0, packet.Length);
                await Task.Delay(50);
                // Receção da pub key do servidor
                size = await netStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size);

                plainText = CryptFunctions.AESDecrypt(response);
            } 
            else
            {
                plainText = serverPubKey;
            }

            if (plainText != string.Empty)
            {
                serverPubKey = plainText;
                string passHash = CryptFunctions.genPassHash(password);
                List<string> register = prepareRegister(username, passHash, email);
                
                await sendRegister(register);

                size = await netStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                
                response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size);
                plainText = CryptFunctions.decryptText(response, privKey);

                changeForm(plainText);
            }
        }

        public static void ReceiveNetworkStream(NetworkStream Stream)
        {
            netStream = Stream;
        }

        private List<string> prepareRegister(string user, string password, string email)
        {
            string heading = "Register";
            string eot = "EOT";
            List<string> preparedMessage = new List<string>();
            preparedMessage.Add(heading);
            preparedMessage.Add(user);
            preparedMessage.Add(password);
            preparedMessage.Add(email);
            preparedMessage.Add(eot);
            return preparedMessage;
        }
        private async Task sendRegister(List<string> preparedMessage)
        {
            try
            {
                foreach (string part in preparedMessage)
                {
                    string cryptPart = CryptFunctions.encryptText(part, serverPubKey);
                    Console.WriteLine("msg " + cryptPart);

                    string hash = CryptFunctions.GenHash(part);
                    Console.WriteLine("Hash " + hash);

                    string signPart = CryptFunctions.SignedHash(hash, privKey);
                    Console.WriteLine("Signature " + signPart);

                    byte[] packetMsg = protocolSI.Make(ProtocolSICmdType.USER_OPTION_3, cryptPart);
                    await netStream.WriteAsync(packetMsg, 0, packetMsg.Length);
                    await Task.Delay(100);

                    byte[] packetHash = protocolSI.Make(ProtocolSICmdType.USER_OPTION_3, hash);
                    await netStream.WriteAsync(packetHash, 0, packetHash.Length);
                    await Task.Delay(100);

                    byte[] packetSign = protocolSI.Make(ProtocolSICmdType.USER_OPTION_3, signPart);
                    await netStream.WriteAsync(packetSign, 0, packetSign.Length);
                    await Task.Delay(100);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: " + ex.Message);
            }
        }
    }
}
