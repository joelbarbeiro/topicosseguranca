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
using System.Data.Entity.Core.Metadata.Edm;

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
                        List<string> data = new List<string>();
                        string msg;
                        int bytesRead;
                        do
                        {
                            // Read data from the network stream
                            bytesRead = netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                            // Extract the data from the buffer
                            protocolSI.SetBuffer(protocolSI.Buffer);
                            string decryptedPart = Encoding.UTF8.GetString(protocolSI.Buffer, 0, bytesRead);
                            msg = CryptFunctions.decryptText(decryptedPart, privKey);
                            // Add the decrypted message part to the data list
                            data.Add(msg);
                            Console.WriteLine("RECEIVED MESSAGE: " + msg);
                            // Check for the "EOT" message to stop reading
                        } while (msg != "EOT" || bytesRead == 0);


                        Console.WriteLine(data.First());

                        switch (data[0])
                        {

                            case "Message":
                                if (data[0] == "Message")
                                {
                                    MessageChat messageChat = new MessageChat(data[1], data[2], data[3]);
                                    // FAZ UPDATE A LISTA DE MENSAGENS NA THREAD DA UI
                                    Invoke((MethodInvoker)delegate
                                    {
                                        Messages.Add(messageChat);
                                        updateMessageList();
                                    });
                                    data.Clear();
                                }

                                break;
                            case "UserList":
                                if (data[0] == "UserList")
                                {
                                    List<string> UserList = new List<string>();

                                    Console.WriteLine("A SLATAR COMO MALUCA");
                                    foreach (string u in data)
                                    {
                                        if (u != "UserList" && u != "EOT" && u != "Message")
                                        {
                                            UserList.Add(u);

                                        }

                                    }
                                    Invoke((MethodInvoker)delegate
                                    {
                                        updateUserList(UserList);
                                    });
                                    data.Clear();
                                }

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
            //string receiver = listBoxUserList.SelectedItem.ToString();
            string receiver = "Scream";
            string msg = textBoxMessage.Text;
            List<string> prepedMessage = new List<string>();
            prepedMessage = prepareMessage(msg, receiver);
            sendMessage(prepedMessage);
            textBoxMessage.Clear();
        }
        private List<string> prepareMessage(string message, string _receiver)
        {
            string heading = "Message";
            string sender = user;
            string receiver = _receiver;
            string bodyMessage = message;
            string eot = "EOT";
            List<string> preparedMessage = new List<string>();
            preparedMessage.Add(heading);
            preparedMessage.Add(receiver);
            preparedMessage.Add(sender);
            preparedMessage.Add(bodyMessage);
            preparedMessage.Add(eot);
            return preparedMessage;
        }

        private void sendMessage(List<string> preparedMessage)
        {

            try
            {
                string cryptPart;
                foreach (string part in preparedMessage)
                {
                    Console.WriteLine($"{part}");
                    cryptPart = CryptFunctions.encryptText(part, serverPubKey);
                    byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, cryptPart);
                    netStream.Write(packet, 0, packet.Length);
                    Thread.Sleep(100); //ADICIONA UM TEMPO DE ESPERA SENAO NAO FUNCIONA
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ALGO ERRADO NAO CERTO" + ex);
            }
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


