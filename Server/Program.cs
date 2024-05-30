using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Threading;
using System.Net.Http;
using System.Xml.Linq;
using Server.models;
using EI.SI;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Data.Entity.Migrations;

namespace Server
{
    internal class Program
    {
        private static List<User> Users;
        public static List<Client> Clients;
        private const int Port = 10000;
        private static string pubKey;
        private static string privKey;

        static void Main(string[] args)
        {
            Users = new List<User>();
            Clients = new List<Client>();

            //ESCREVER PARA CONSOLA
            Console.WriteLine("A iniciar o servidor...");
            //DECLARAÇÃO DE VARIAVEIS DE SISTEMA PARA IP E TCPLISTENER
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
            TcpListener listener = new TcpListener(endPoint);
            CryptControllers.keyGen(out pubKey, out privKey);
            listener.Start();
            Console.WriteLine("SERVER READY!");
            int clientCounter = 0;
            //CICLO INFINITO PARA ESTAR SEMPRE À ESCUTA DE CLIENTES A CONECTAREM-SE
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clientCounter++;
                Console.WriteLine("Client {0} connected", clientCounter);
                ClientHandler clientHandler = new ClientHandler(client, clientCounter);
                clientHandler.Handle();
            }
        }

        //CLASSE PARA TRATAMENTO DE DADOS DO CLIENTE COM THREADS
        class ClientHandler
        {
            private TcpClient client;
            private int clientID;
            public ClientHandler(TcpClient client, int clientID)
            {
                this.client = client;
                this.clientID = clientID;
            }
            public void Handle()
            {
                Thread thread = new Thread(threadHandler);
                thread.Start();
            }

            private void threadHandler()
            {
                string clientPubKey = null;
                NetworkStream networkStream = this.client.GetStream();
                ProtocolSI protocolSI = new ProtocolSI();
                while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] sendAck = null;
                    string ack = null;
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.USER_OPTION_1:
                            // RECEBE A PUBLIC KEY
                            clientPubKey = CryptControllers.AESDecrypt(protocolSI.GetStringFromData());
                            ack = CryptControllers.AESEncrypt(Program.pubKey);
                            sendAck = Encoding.UTF8.GetBytes(ack);
                            networkStream.Write(sendAck, 0, sendAck.Length);
                            break;
                        case ProtocolSICmdType.USER_OPTION_2:
                            // CONTROLO DO LOGIN 
                            Console.WriteLine(clientPubKey);
                            Console.WriteLine("ENTREI AQUII ANTES DO WHILE");
                            List<string> dataLogin = new List<string>();
                            List<string> usersToList = new List<string>();
                            string loginField = CryptControllers.decryptText(protocolSI.GetStringFromData(), privKey);
                            dataLogin.Add(loginField);
                            while (true)
                            {
                                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                                if (bytesRead == 0)
                                {
                                    break;
                                }

                                protocolSI.SetBuffer(protocolSI.Buffer); // Adjust to correctly set the buffer
                                string encryptedPart = protocolSI.GetStringFromData();
                                string decryptedPart = CryptControllers.decryptText(encryptedPart, privKey);

                                dataLogin.Add(decryptedPart);


                                if (decryptedPart == "EOT")
                                {
                                    break;
                                }
                            }
                            foreach (string part in dataLogin)
                            {
                                Console.WriteLine("FOREACH DEPOIS DO WHILE  " + part);
                            }

                            // Validate login data
                            if (dataLogin.Count >= 4 && dataLogin[0] == "Login")
                            {
                                string username = dataLogin[1];
                                string password = dataLogin[2];

                                if (ValidationControllers.authUser(username, password))
                                {
                                    ack = HandleControllers.handleLogin(dataLogin);
                                    ack = CryptControllers.encryptText(ack, clientPubKey);
                                    sendAck = Encoding.UTF8.GetBytes(ack);
                                    networkStream.Write(sendAck, 0, sendAck.Length);
                                    Client clientC = new Client(client, dataLogin[1], clientPubKey);
                                    Clients.Add(clientC);
                                    usersToList = HandleControllers.handleListUsers();
                                    BroadcastMessage(usersToList);
                                    LogControllers.createLog(dataLogin[0], dataLogin[1], "Success");
                                    // Additional logic for handling authenticated users*/
                                }
                                else
                                {
                                    LogControllers.createLog(dataLogin[0], dataLogin[1], "Failed");
                                }
                            }

                            break;
                        case ProtocolSICmdType.USER_OPTION_3:
                            // CONTROLO DO REGISTER
                            string getRegister = protocolSI.GetStringFromData();
                            string[] register = CryptControllers.decryptText(getRegister, privKey).Split('ᚼ');
                            string response = HandleControllers.handleRegister(register);
                            ack = CryptControllers.encryptText(response, clientPubKey);
                            sendAck = Encoding.UTF8.GetBytes(ack);
                            networkStream.Write(sendAck, 0, sendAck.Length);
                            LogControllers.createLog(register[0], register[1]);
                            break;
                        case ProtocolSICmdType.DATA:
                            List<string> data = new List<string>();
                            bool isReading = true;
                            string messageField = CryptControllers.decryptText(protocolSI.GetStringFromData(), privKey);
                            data.Add(messageField);
                            while (isReading)
                            {
                                // Read data from the network stream
                                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                                // If bytesRead is zero, exit the loop
                                if (bytesRead == 0)
                                {
                                    break;
                                }

                                // Extract the data from the buffer
                                protocolSI.SetBuffer(protocolSI.Buffer);
                                string decryptedPart = protocolSI.GetStringFromData();
                                string msg = CryptControllers.decryptText(decryptedPart, privKey);

                                // Add the decrypted message part to the data list
                                data.Add(msg);
                                Console.WriteLine("MESSAGE: " + msg);

                                // Check for the "EOT" message to stop reading
                                if (msg == "EOT")
                                {
                                    isReading = false;
                                }
                            }

                            BroadcastMessage(data);
                            // LogControllers.createLog(data[0], data[1], data[2]);

                            break;
                        case ProtocolSICmdType.EOT:
                            //CONTROLO DO END OF TRANSMISSION
                            List<string> usersToRemove = new List<string>();
                            Console.WriteLine("Ending Thread from Client" + "{0}", clientID);
                            protocolSI.GetStringFromData();
                            sendAck = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(sendAck, 0, sendAck.Length);
                            HandleControllers.removeUserFromList(client);
                            usersToRemove = HandleControllers.handleListUsers();
                            //BroadcastMessage(ack, "Scream");
                            break;
                    }
                }
                networkStream.Close();
                client.Close();
                Thread.Sleep(100); //ADICIONA UM TEMPO DE ESPERA 

            }

        }
        //FUNÇÃO PARA UPDATE DO ESTADO DO UTILIZADOR SE ESTA LIGADO OU NÃO



        public static void updateStateLoggin(int id, bool state)
        {
            serverContext context = new serverContext();
            var user = context.Users.SingleOrDefault(User => User.IdUser == id);
            if (user != null)
            {
                user.isLoggedIn = state;
                context.SaveChanges();
            }
        }
        //FUNÇÃO PARA MANDAR A MENSAGEM PARA TODOS OS CLIENTES
        private static void BroadcastMessage(List<string> preparedMessage)
        {

            try
            {
                if (preparedMessage.Contains("Scream"))
                {
                    foreach (Client client in Clients)
                    {
                        string cryptPart;
                        foreach (string part in preparedMessage)
                        {
                            Console.WriteLine($"{part} BROADCASTTTT");
                            cryptPart = CryptControllers.encryptText(part, client.PublicKey);
                            byte[] packet = Encoding.UTF8.GetBytes(cryptPart);
                            NetworkStream stream = client.TcpClient.GetStream();
                            stream.Write(packet, 0, packet.Length);
                            Thread.Sleep(100); //ADICIONA UM TEMPO DE ESPERA SENAO NAO FUNCIONA
                        }
                    }
                    preparedMessage.Clear();
                }
                else
                {
                    foreach (Client client in Clients)
                    {
                        if (preparedMessage.Contains(client.User))
                        {
                            string cryptPart;

                            foreach (string part in preparedMessage)
                            {
                                Console.WriteLine($"{part} WHISPER");
                                cryptPart = CryptControllers.encryptText(part, client.PublicKey);
                                byte[] packet = Encoding.UTF8.GetBytes(cryptPart);
                                NetworkStream stream = client.TcpClient.GetStream();
                                stream.Write(packet, 0, packet.Length);
                                Thread.Sleep(100); //ADICIONA UM TEMPO DE ESPERA SENAO NAO FUNCIONA
                            }
                        }
                    }
                    preparedMessage.Clear();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ALGO ERRADO NAO CERTO" + ex);
            }
        }

    }


    //SE O RECEIVER FOR NULL ELE MANDA PARA TODOS OS CLIENTES
    /*if (Receiver == null)
    {
        foreach (Client client in Clients)
        {
            string cryptMessage = CryptControllers.encryptText(message, client.PublicKey);
            byte[] sendMessage = Encoding.UTF8.GetBytes(cryptMessage);
            NetworkStream stream = client.TcpClient.GetStream();
            stream.Write(sendMessage, 0, sendMessage.Length);
        }
    }
    else
    {
        // SE O RECEIVER NAO FOR NULL ELE VAI PROCURAR O USER COM O MESMO NOME NA LISTA CLIENTS
        foreach (Client client in Clients)
        {
            if (Sender == client.User || Receiver == client.User)
            {
                string cryptMessage = CryptControllers.encryptText(message, client.PublicKey);
                byte[] sendMessage = Encoding.UTF8.GetBytes(cryptMessage);
                NetworkStream stream = client.TcpClient.GetStream();
                stream.Write(sendMessage, 0, sendMessage.Length);
            }
        }
    }*/
}

