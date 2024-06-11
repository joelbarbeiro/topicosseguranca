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
using System.Data.Entity.Core.Metadata.Edm;
using System.Runtime.InteropServices.ComTypes;
using System.IO.Ports;

namespace Server
{
    internal class Program
    {
        private static List<User> Users;
        public static List<Client> Clients;
        private const int Port = 10000;
        private static string pubKey;
        private static string privKey;
        public static bool running = true;

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

            private async void threadHandler()
            {
                string clientPubKey = null;
                NetworkStream networkStream = this.client.GetStream();
                ProtocolSI protocolSI = new ProtocolSI();
                while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    running = true;
                    await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] sendAck = null;
                    string ack = null;

                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.USER_OPTION_1:
                            // RECEBE A PUBLIC KEY
                            clientPubKey = CryptControllers.AESDecrypt(protocolSI.GetStringFromData());

                            ack = CryptControllers.AESEncrypt(Program.pubKey);
                            sendAck = Encoding.UTF8.GetBytes(ack);

                            await networkStream.WriteAsync(sendAck, 0, sendAck.Length);


                            break;

                        case ProtocolSICmdType.USER_OPTION_2:
                            // CONTROLO DO LOGIN 
                            Console.WriteLine(clientPubKey);

                            List<string> dataLogin = new List<string>();
                            List<string> usersToList = new List<string>();

                            dataLogin = await ReadNetstreamParts(networkStream, protocolSI, clientPubKey);

                            // Validate login data
                            if (dataLogin.Count() > 0)
                            {
                                if (dataLogin.Count >= 4 && dataLogin[0] == "Login")
                                {
                                    string username = dataLogin[1];
                                    string password = dataLogin[2];


                                    if (ValidationControllers.authUser(username, password))
                                    {
                                        ack = HandleControllers.handleLogin(dataLogin);
                                        Console.WriteLine("State login -- > " + ack);
                                        string encryptedAck = CryptControllers.encryptText(ack, clientPubKey);
                                        sendAck = Encoding.UTF8.GetBytes(encryptedAck);

                                        await networkStream.WriteAsync(sendAck, 0, sendAck.Length);
                                        Client clientC = new Client(client, dataLogin[1], clientPubKey);
                                        Clients.Add(clientC);

                                        await Task.Delay(100);

                                        usersToList = HandleControllers.handleListUsers();
                                        await BroadcastMessage(usersToList);
                                        LogControllers.createLog(dataLogin[0], dataLogin[1], "Success");
                                    }
                                    else
                                    {
                                        ack = HandleControllers.handleLogin(dataLogin);
                                        Console.WriteLine("State login -- > " + ack);
                                        string encryptedAck = CryptControllers.encryptText(ack, clientPubKey);
                                        sendAck = Encoding.UTF8.GetBytes(encryptedAck);

                                        await networkStream.WriteAsync(sendAck, 0, sendAck.Length);
                                        LogControllers.createLog(dataLogin[0], dataLogin[1], "Failed");
                                    }
                                }
                            }
                            dataLogin.Clear();
                            break;

                        case ProtocolSICmdType.USER_OPTION_3:
                            // CONTROLO DO REGISTER
                            List<string> dataRegister = new List<string>();

                            dataRegister = await ReadNetstreamParts(networkStream, protocolSI, clientPubKey);

                            if (dataRegister.Count() > 0)
                            {
                                string response = HandleControllers.handleRegister(dataRegister);
                                ack = CryptControllers.encryptText(response, clientPubKey);
                                sendAck = Encoding.UTF8.GetBytes(ack);

                                await Task.Delay(100);

                                await networkStream.WriteAsync(sendAck, 0, sendAck.Length);

                                LogControllers.createLog(dataRegister[0], dataRegister[1]);
                            }
                            dataRegister.Clear();
                            break;

                        case ProtocolSICmdType.DATA:

                            List<string> dataMessage = new List<string>();

                            dataMessage = await ReadNetstreamParts(networkStream, protocolSI, clientPubKey);
                            if (dataMessage.Count() > 0)
                            {
                                LogControllers.createLog(dataMessage[0], dataMessage[1], dataMessage[2]);

                                await BroadcastMessage(dataMessage);
                            }
                            break;

                        case ProtocolSICmdType.EOT:
                            //CONTROLO DO END OF TRANSMISSION
                            List<string> usersToRemove = new List<string>();
                            Console.WriteLine("Ending Thread from Client" + "{0}", clientID);

                            protocolSI.GetStringFromData();
                            sendAck = protocolSI.Make(ProtocolSICmdType.ACK);
                            await networkStream.WriteAsync(sendAck, 0, sendAck.Length);

                            HandleControllers.removeUserFromList(client);
                            usersToRemove = HandleControllers.handleListUsers();

                            await BroadcastMessage(usersToRemove);
                            break;
                    }
                    await Task.Delay(100);
                }
                networkStream.Close();
                client.Close();
            }
        }

        private async static Task<string> ReadMessage(NetworkStream networkStream, ProtocolSI protocolSI)
        {
            await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            protocolSI.SetBuffer(protocolSI.Buffer);

            return protocolSI.GetStringFromData();
        }

        private async static Task<List<String>> ReadNetstreamParts(NetworkStream networkStream, ProtocolSI protocolSI, string clientPubKey)
        {
            List<String> parts = new List<String>();
            try
            {
                while (running)
                {

                    protocolSI.SetBuffer(protocolSI.Buffer);
                    string encryptedPart = protocolSI.GetStringFromData();

                    string decryptedMsg = CryptControllers.decryptText(encryptedPart, privKey);

                    string hash = await ReadMessage(networkStream, protocolSI);

                    string signHash = await ReadMessage(networkStream, protocolSI);

                    bool val = CryptControllers.VerifyHash(signHash, hash, clientPubKey);
                    if (val)
                    {
                        Console.WriteLine("Mensagem verificada");
                    }
                    else
                    {
                        Console.WriteLine("Mensagem avariada!");
                        parts.Clear();
                        running = false;
                        break;
                    }

                    parts.Add(decryptedMsg);

                    if (decryptedMsg == "EOT")
                    {
                        running = false;
                        break;
                    }

                    int bytesLoginRead = await networkStream.ReadAsync(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                    if (bytesLoginRead == 0)
                    {
                        running = false;
                        break;
                    }
                    Console.WriteLine("running");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading message " + ex);
                running = false;
            }
            finally
            {
                Console.WriteLine("Ends read message");
            }

            return parts;
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
        private static async Task BroadcastMessage(List<string> preparedMessage)
        {

            try
            {
                if (preparedMessage.Contains("Scream"))
                {
                    foreach (Client client in Clients)
                    {
                        foreach (string part in preparedMessage)
                        {
                            Console.WriteLine($"{part} BROADCASTTTT");

                            string cryptMessage = CryptControllers.encryptText(part, client.PublicKey);
                            byte[] sendMessage = Encoding.UTF8.GetBytes(cryptMessage);
                            NetworkStream stream = client.TcpClient.GetStream();

                            await stream.WriteAsync(sendMessage, 0, sendMessage.Length);
                            //await SendMessageAsync(stream, sendMessage);
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
                            foreach (string part in preparedMessage)
                            {
                                Console.WriteLine($"{part} WHISPER");

                                string cryptMessage = CryptControllers.encryptText(part, client.PublicKey);
                                byte[] sendMessage = Encoding.UTF8.GetBytes(cryptMessage);
                                NetworkStream stream = client.TcpClient.GetStream();
                                await stream.WriteAsync(sendMessage, 0, sendMessage.Length);
                                //await SendMessageAsync(stream, sendMessage);
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
        private static async Task SendMessageAsync(NetworkStream stream, byte[] message)
        {
            const int PacketSize = 1024;
            int totalPackets = (message.Length + PacketSize - 1) / PacketSize;

            for (int i = 0; i < totalPackets; i++)
            {
                int start = i * PacketSize;
                int length = Math.Min(PacketSize, message.Length - start);
                byte[] packet = new byte[length];
                Array.Copy(message, start, packet, 0, length);
                await stream.WriteAsync(packet, 0, packet.Length);
            }
        }

    }
}

