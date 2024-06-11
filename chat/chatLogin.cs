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
using chat.models;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Security.Cryptography;

namespace chat
{
    public partial class FormChatLogin : Form
    {

        private const int Port = 10000;
        public static NetworkStream netStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;
        public string user;
        public string pubKey;
        public string privKey;
        public string serverPubKey = string.Empty;


        public FormChatLogin()
        {


            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, Port);
            CryptFunctions.keyGen(out pubKey, out privKey);

            TcpClient tcpClient = new TcpClient();

            if (tcpClient.Connected == false)
            {
                tcpClient.Connect(endPoint);
            }
            netStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();
            FormChatLogin.ReceiveNetworkStream(netStream);


            InitializeComponent();
            protocolSI = new ProtocolSI();

        }

        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            string response;
            string plainText = string.Empty;
            int size;



            if (serverPubKey == string.Empty)
            {
                Console.WriteLine("Client pub key " + pubKey);
                string msg = CryptFunctions.AESEncrypt(pubKey);

                // Envio da public key do cliente
                byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, msg);
                await netStream.WriteAsync(packet, 0, packet.Length);

                // Receção da pub key do servidor
                size = await netStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size);

                plainText = CryptFunctions.AESDecrypt(response);
                Console.WriteLine("Server Pub key " + plainText);
            }
            else
            {
                plainText = serverPubKey;
            }
            if (plainText != string.Empty)
            {
                serverPubKey = plainText;

                // Preparação dos dados para login
                string user = textBoxLoginUsername.Text;
                string password = CryptFunctions.genPassHash(textBoxLoginPassword.Text);
                List<string> login = new List<string>();
                login = prepareLogin(user, password);

                // envio dos dados de login encriptados para o servidor
                await sendLogin(login);

                login.Clear();
                size = await netStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size);

                plainText = CryptFunctions.decryptText(response, privKey);

                Console.WriteLine(plainText);
                changeForm(plainText);
            }
        }

        private void textBoxLoginUsername_Enter(object sender, EventArgs e)
        {
            textBoxLoginUsername.Text = "";
            loginButtonControl();
        }
        private void textBoxLoginUsername_Leave(object sender, EventArgs e)
        {
            if (textBoxLoginUsername.Text.Length == 0)
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
            if (response == "OK")
            {
                Console.WriteLine(response);
                user = textBoxLoginUsername.Text;
                FormChat mainForm = new FormChat(user, tcpClient, netStream, privKey, pubKey, serverPubKey);
                FormChat.ReceiveNetworkStream(netStream);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                Console.WriteLine(response);
                MessageBox.Show("Login failed. Please try again.");
            }
        }

        private void buttonChatRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            formChatRegister chatRegisterForm = new formChatRegister(tcpClient, netStream, privKey, pubKey, serverPubKey);
            chatRegisterForm.ShowDialog();
        }

        private async void CloseClient()
        {
            byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
            await netStream.WriteAsync(eot, 0, eot.Length);
            await netStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
        }
        private void openClient(NetworkStream netStream)
        {
            netStream = tcpClient.GetStream();
        }

        private void FormChatLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseClient();
        }


        public static void ReceiveNetworkStream(NetworkStream Stream)
        {
            netStream = Stream;
        }

        private List<string> prepareLogin(string user, string password)
        {
            string heading = "Login";
            string eot = "EOT";
            List<string> preparedMessage = new List<string>();
            preparedMessage.Add(heading);
            preparedMessage.Add(user);
            preparedMessage.Add(password);
            preparedMessage.Add(eot);
            return preparedMessage;
        }

        private async Task sendLogin(List<string> preparedMessage)
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

                    byte[] packetMsg = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, cryptPart);
                    await netStream.WriteAsync(packetMsg, 0, packetMsg.Length);
                    await Task.Delay(100);

                    byte[] packetHash = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, hash);
                    await netStream.WriteAsync(packetHash, 0, packetHash.Length);
                    await Task.Delay(100);

                    byte[] packetSign = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, signPart);
                    await netStream.WriteAsync(packetSign, 0, packetSign.Length);
                    await Task.Delay(100);

                }
                preparedMessage.Clear();
            }
            catch (Exception ex)
            {
                preparedMessage.Clear();
                MessageBox.Show("Something went wrong: " + ex.Message);
            }
        }

    }
}