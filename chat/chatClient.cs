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
using chat.models;
using EI.SI;
using System.Threading;
using static chat.FormChatLogin;

namespace chat
{
    public partial class FormChat : Form
    {

        private List<MessageChat> Messages;
        private const int Port = 10000;
        public static NetworkStream netStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;
        private string user;
        public string pubKey;
        public string privKey;
        public string serverPubKey;

        public FormChat(string userName, TcpClient tcpClient, NetworkStream netStream, string privKey, string pubKey, string serverPubKey)
        {
            this.user = userName;
            this.tcpClient = tcpClient;
            this.privKey = privKey;
            this.pubKey = pubKey;
            this.serverPubKey = serverPubKey;

            InitializeComponent();
            protocolSI = new ProtocolSI();
            Messages = new List<MessageChat>();
            user = userName;
            ReceiveNetworkStream(netStream);
            Handler();
        }

        private void Handler()
        {
            Thread thread = new Thread(getMessages);
            thread.Start();
        }

        private void getMessages()
        {

            while (true) // LOOP PARA LER MENSAGENS INFINITO
            {
                try
                {
                    if (netStream.DataAvailable) // VERIFICA SE TEM DATA DISPONIVEL
                    {
                        int size = netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                        string response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size);
                        string plainText = CryptFunctions.decryptText(response, privKey);
                        string[] argUser = plainText.Split('-');
                        switch (argUser[0])
                        {
                            case "Message":
                                MessageChat messageChat = new MessageChat(argUser[1], argUser[2], argUser[3]);
                                // FAZ UPDATE A LISTA DE MENSAGENS NA THREAD DA UI
                                Invoke((MethodInvoker)delegate {
                                    Messages.Add(messageChat);
                                    updateMessageList();
                                });
                                break;
                            case "UserList":
                                List<string> UserList = new List<string>();
                                foreach (string u in argUser)
                                {
                                    if (u != "UserList")
                                    {
                                        UserList.Add(u);
                                    }
                                    else
                                    {
                                        UserList.Add("Scream");
                                    }
                                }
                                Invoke((MethodInvoker)delegate
                                {
                                    updateUserList(UserList);
                                });
                                break;
                        }
                    }
                    Thread.Sleep(100); //ADICIONA UM TEMPO DE ESPERA 
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    break;
                }
            }
        }
        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            string type = listBoxUserList.SelectedItem.ToString();
            string msg = "Message-" + type + "-" + user + "-" + textBoxMessage.Text;
            string cryptedText = CryptFunctions.encryptText(msg, serverPubKey);
            textBoxMessage.Clear();
            byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_4, cryptedText);
            netStream.Write(packet, 0, packet.Length);

        }

        private void updateMessageList()
        {
            listBoxMessage.DataSource = null;
            listBoxMessage.DataSource = Messages;
        }
        private void updateUserList(List<String> UserList)
        {
            listBoxUserList.DataSource = null;
            listBoxUserList.DataSource = UserList;
        }

        private void FormChat_Load(object sender, EventArgs e)
        {
            labelUserName.Text = user;
        }

        private void buttonChatLogout_Click(object sender, EventArgs e)
        {
            FormChatLogin formchatlogin = new FormChatLogin(tcpClient, pubKey, privKey, serverPubKey);
            formchatlogin.Show();
            this.Close();

        }

        public static void ReceiveNetworkStream(NetworkStream Stream)
        {
            netStream = Stream;
        }
    }
}


