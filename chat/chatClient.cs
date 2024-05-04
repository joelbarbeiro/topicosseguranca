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

        public FormChat(string userName, TcpClient tcpClient,NetworkStream netStream)
        {
            InitializeComponent();
            protocolSI = new ProtocolSI();
            Messages = new List<MessageChat>();
            user = userName;
            ReceiveNetworkStream(netStream);
            Handler();
            if (tcpClient == null || !tcpClient.Connected)
            {
                //InitializeTcpClient();

            }
        }

        private void InitializeTcpClient()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, Port);
                tcpClient = TcpClientSingleton.GetInstance();
                tcpClient.Connect(endPoint);
                netStream = tcpClient.GetStream();
                protocolSI = new ProtocolSI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
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
                        string[] response;
                        int size;
                        size = netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                        response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size).Split('-');
                        switch (response[0]) {
                            case "Message":
                                MessageChat messageChat = new MessageChat(response[1], response[2], response[3]);
                            // FAZ UPDATE A LISTA DE MENSAGENS NA THREAD DA UI
                            Invoke((MethodInvoker)delegate {
                                Messages.Add(messageChat);
                                updateMessageList();
                            });
                            break;
                            case "UserList":
                                List<string> UserList = new List<string>();
                                foreach (string u in response)
                                {
                                    if(u != "UserList")
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
            textBoxMessage.Clear();
            byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_3, msg);
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

        private void CloseClient()
        {
            byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
            netStream.Write(eot, 0, eot.Length);
            netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            netStream.Close();
            tcpClient.Close();
        }


        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseClient();
            FormChatLogin formchatlogin = new FormChatLogin(tcpClient);
            formchatlogin.Close();
        }

        private void FormChat_Load(object sender, EventArgs e)
        {
            labelUserName.Text = user;
        }

        private void buttonChatLogout_Click(object sender, EventArgs e)
        {
            FormChatLogin formchatlogin = new FormChatLogin(tcpClient);
            formchatlogin.Show();
            this.Close();

        }

        public static void ReceiveNetworkStream(NetworkStream Stream)
        {
            netStream=Stream;
        }
    }
}


