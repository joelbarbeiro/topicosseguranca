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


        static void Main(string[] args)
        {
            Users = new List<User>();
            Clients = new List<Client>();

            //ESCREVER PARA CONSOLA
            Console.WriteLine("A iniciar o servidor...");
            //DECLARAÇÃO DE VARIAVEIS DE SISTEMA PARA IP E TCPLISTENER
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
            TcpListener listener = new TcpListener(endPoint);
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
                string pubKey = null;
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
                            pubKey = protocolSI.GetStringFromData();
                            ack = CryptControllers.encryptText("Ok", pubKey);
                            sendAck = Encoding.UTF8.GetBytes(ack);
                            networkStream.Write(sendAck, 0, ack.Length);
                            break;

                        case ProtocolSICmdType.USER_OPTION_2:
                            // CONTROLO DO LOGIN 
                            string getLogin = protocolSI.GetStringFromData();
                            string[] login = CryptControllers.decryptText(getLogin, pubKey).Split('-');
                            Console.WriteLine(login[1] + ": " + login[2] + "LOGIN!!");
                            string val = HandleControllers.handleLogin(login);
                            ack = CryptControllers.encryptText(val, pubKey);
                            sendAck = Encoding.UTF8.GetBytes(ack);
                            networkStream.Write(sendAck, 0, ack.Length);

                            if (ValidationControllers.authUser(login[1], login[2]))
                            {
                                Client clientC = new Client(client, login[1], pubKey);
                                Clients.Add(clientC);
                                ack = HandleControllers.handleListUsers(); //12 bytes
                                int size = ack.Length;
                                Console.WriteLine("Size of byte stream: " + size + " bytes");

                                BroadcastMessage(ack, "Scream");
                                LogControllers.createLog(login[0], login[1], "Success");
                            }
                            else
                            {
                                LogControllers.createLog(login[0], login[1], "Failed");
                            }
                            break;
                        case ProtocolSICmdType.USER_OPTION_3:
                            // CONTROLO DO REGISTER
                            string getRegister = protocolSI.GetStringFromData();
                            string[] register = CryptControllers.decryptText(getRegister, pubKey).Split('-');
                            Console.WriteLine(register[0] + ": " + register[1] + ": " + register[2] + ": " + register[3]);
                            string response = HandleControllers.handleRegister(register);
                            ack = CryptControllers.encryptText(ack, pubKey);
                            sendAck = Encoding.UTF8.GetBytes(ack);
                            networkStream.Write(sendAck, 0, ack.Length);
                            LogControllers.createLog(register[0], register[1]);
                            break;
                        case ProtocolSICmdType.USER_OPTION_4:
                            // CONTROLO DAS MENSAGENS
                            string userKey = HandleControllers.getKeyFromList(client);
                            string msg = CryptControllers.decryptText(protocolSI.GetStringFromData(), userKey);
                            string[] message = msg.Split('-');
                            Console.WriteLine(message[1] + ": " + message[2] + "MESSAGEUO4");
                            //ack = handleMessage(message);
                            BroadcastMessage(HandleControllers.handleMessage(message), message[1], message[2]);
                            LogControllers.createLog(message[0], message[1], message[2]);
                            break;
                        case ProtocolSICmdType.EOT:
                            //CONTROLO DO END OF TRANSMISSION
                            Console.WriteLine("Ending Thread from Client" + "{0}", clientID);
                            protocolSI.GetStringFromData();
                            sendAck = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(sendAck, 0, ack.Length);
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
        private static void BroadcastMessage(string message, string userI, string userO = null)
        {
            if (userI == "Scream")
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
                foreach (Client client in Clients)
                {
                    if (userI == client.User || userO == client.User)
                    {
                        string cryptMessage = CryptControllers.encryptText(message, client.PublicKey);
                        byte[] sendMessage = Encoding.UTF8.GetBytes(cryptMessage);
                        NetworkStream stream = client.TcpClient.GetStream();
                        stream.Write(sendMessage, 0, sendMessage.Length);
                    }
                }
            }

        }
        
        
    }
}