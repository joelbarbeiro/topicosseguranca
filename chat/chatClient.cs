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
using System.Text.RegularExpressions;

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
        public bool isRunning = true;

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

        private async void getMessages()
        {
            while (isRunning) // LOOP PARA LER MENSAGENS INFINITO
            {
                try
                {

                    if (netStream.DataAvailable) // VERIFICA SE TEM DATA DISPONIVEL
                    {
                        List<string> data = new List<string>();
                        string msg;
                        int bytesRead;
                        int i = 0;

                        bytesRead = await netStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                        string decryptedPart = Encoding.UTF8.GetString(protocolSI.Buffer, 0, bytesRead);

                        string[] splitParts = decryptedPart.Split('=');
                        int numberParts = splitParts.Count();
                        Console.WriteLine("Num parts " + numberParts);
                        while (i < numberParts - 1)
                        {
                            msg = CryptFunctions.decryptText(splitParts[i] + '=', privKey);

                            Console.WriteLine(msg);
                            data.Add(msg);
                            i++;
                        }

                        Console.WriteLine("O que vai fazer --> " + data.First());

                        switch (data[0])
                        {
                            case "Message":
                                MessageChat messageChat = new MessageChat(data[1], data[2], data[3]);
                                // FAZ UPDATE A LISTA DE MENSAGENS NA THREAD DA UI
                                Invoke((MethodInvoker)delegate
                                {
                                    Messages.Add(messageChat);
                                    updateMessageList();
                                });
                                data.Clear();

                                break;
                            case "UserList":
                                List<string> UserList = new List<string>();

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
                finally
                {
                    Console.WriteLine("Continua");
                    getMessages();
                }

            }
        }
        private async Task<List<String>> ReadNetstreamParts(NetworkStream networkStream, ProtocolSI protocolSI, string clientPubKey)
        {
            List<String> parts = new List<String>();
            while (true)
            {
                int bytesLoginRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                if (bytesLoginRead == 0)
                {
                    break;
                }
                string encryptedPart = await ReadMessage(networkStream, protocolSI);

                string decryptedMsg = CryptFunctions.decryptText(encryptedPart, privKey);

                parts.Add(decryptedMsg);

                if (decryptedMsg == "EOT")
                {
                    break;
                }



            }
            return parts;
        }

        private async static Task<string> ReadMessage(NetworkStream networkStream, ProtocolSI protocolSI)
        {
            await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            protocolSI.SetBuffer(protocolSI.Buffer);

            return protocolSI.GetStringFromData();
        }

        private async void buttonSendMessage_Click(object sender, EventArgs e)
        {
            string receiver = listBoxUserList.SelectedItem.ToString();
            string msg = textBoxMessage.Text;
            List<string> prepedMessage = new List<string>();
            prepedMessage = prepareMessage(msg, receiver);
            await sendMessage(prepedMessage);
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

        private async Task sendMessage(List<string> preparedMessage)
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

                    byte[] packetMsg = protocolSI.Make(ProtocolSICmdType.DATA, cryptPart);
                    await netStream.WriteAsync(packetMsg, 0, packetMsg.Length);
                    await Task.Delay(100);

                    byte[] packetHash = protocolSI.Make(ProtocolSICmdType.DATA, hash);
                    await netStream.WriteAsync(packetHash, 0, packetHash.Length);
                    await Task.Delay(100);

                    byte[] packetSign = protocolSI.Make(ProtocolSICmdType.DATA, signPart);
                    await netStream.WriteAsync(packetSign, 0, packetSign.Length);
                    await Task.Delay(100);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ALGO ERRADO NAO CERTO" + ex);
            }
            finally
            {
                Console.WriteLine("keep going");
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
            FormChatLogin formchatlogin = new FormChatLogin();
            formchatlogin.Show();

            this.Close();
        }
        private async void CloseClient()
        {
            isRunning = false;
            byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
            await netStream.WriteAsync(eot, 0, eot.Length);
            await netStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            serverPubKey = string.Empty;

            await Task.Delay(100);
        }

        public static void ReceiveNetworkStream(NetworkStream Stream)
        {
            netStream = Stream;
        }

        private void FormChat_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseClient();
        }
    }
}


