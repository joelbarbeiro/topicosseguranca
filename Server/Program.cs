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
        private static List<Client> Clients;
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
                    byte[] ack = null;
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.USER_OPTION_1:
                            // RECEBE A PUBLIC KEY
                            pubKey = protocolSI.GetStringFromData();
                            Console.WriteLine("PubKey -> " + pubKey);
                            ack = Encoding.UTF8.GetBytes("Ok");
                            networkStream.Write(ack, 0, ack.Length);
                            break;

                        case ProtocolSICmdType.USER_OPTION_2:
                            // CONTROLO DO LOGIN 
                            string[] login = decryptText(protocolSI.GetStringFromData(), pubKey).Split('-');
                            Console.WriteLine(login[1] + ": " + login[2]);
                            ack = handleLogin(login);
                            networkStream.Write(ack, 0, ack.Length);

                            if (authUser(login[1], login[2]))
                            {
                                Client clientC = new Client(client, login[1], pubKey);
                                Clients.Add(clientC);
                                string _ack = handleListUsers();
                                BroadcastMessage(_ack, "Scream");
                                createLog(login[0], login[1], "Success");
                            }
                            else
                            {
                                createLog(login[0], login[1], "Failed");
                            }
                            break;
                        case ProtocolSICmdType.USER_OPTION_3:
                            // CONTROLO DO REGISTER
                            string[] register = protocolSI.GetStringFromData().Split('-');
                            Console.WriteLine(register[0] + ": " + register[1] + ": " + register[2] + ": " + register[3]);
                            ack = handleRegister(register);
                            networkStream.Write(ack, 0, ack.Length);
                            createLog(register[0], register[1]);
                            break;
                        case ProtocolSICmdType.USER_OPTION_4:
                            // CONTROLO DAS MENSAGENS
                            string msg = decryptText(protocolSI.GetStringFromData(), pubKey);
                            string[] message = msg.Split('-');
                            Console.WriteLine(message[1] + ": " + message[2]);
                            //ack = handleMessage(message);
                            BroadcastMessage(handleMessage(message), message[1], message[2]);
                            createLog(message[0], message[1], message[2]);
                            break;
                        case ProtocolSICmdType.EOT:
                            //CONTROLO DO END OF TRANSMISSION
                            Console.WriteLine("Ending Thread from Client" + "{0}", clientID);
                            protocolSI.GetStringFromData();
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
                    }
                }
                networkStream.Close();
                client.Close();
                Thread.Sleep(100); //ADICIONA UM TEMPO DE ESPERA 
            }
        }

        //FUNÇÃO PARA AUTENTICAÇÃO DO UTILIZADOR
        private static bool authUser(string user, string pass)
        {
            serverContext context = new serverContext();
            var query_result_alt = context.Users.Where(
            User =>
            User.user == user &&
            User.password == pass);
            if(query_result_alt.Count() == 0)
            {
               Console.WriteLine("Login Failed");
               return false;
            }
            updateStateLoggin(query_result_alt.First().IdUser, true);
            
            return true;
        }

        //FUNÇÃO PARA UPDATE DO ESTADO DO UTILIZADOR SE ESTA LIGADO OU NÃO
        private static void updateStateLoggin(int id, bool state)
        {
            serverContext context = new serverContext();
            var user = context.Users.SingleOrDefault(User => User.IdUser == id);
            if (user != null)
            {
                user.isLoggedIn = state;
                context.SaveChanges();
            }
        }

        private static bool checkLoggedIn(int id)
        {
            serverContext context = new serverContext();
            var query_result_alt = context.Users.Where(
            User =>
            User.IdUser == id && User.isLoggedIn == true);
            if (query_result_alt.Count() == 0)
            {
                Console.WriteLine("Login Failed");
                return false;
            }
            return true;
        }

        //FUNÇÃO PARA TRATAR DO REGISTO DAS CONTAS
        private static byte[] handleRegister(string[] _data)
        {
            byte[] response = null;
            string user = _data[1];
            string pass = _data[2];
            string email = _data[3];

            if (userConfirmRegister(user, email))
            {
                User new_account = new User(user, pass, email);
                serverContext context = new serverContext();
                context.Users.Add(new_account);

                try
                {
                    context.SaveChanges();
                }
                catch(Exception ex) 
                { 
                    Console.WriteLine("Couldn't save new account   -- "+ex);
                    response = Encoding.UTF8.GetBytes("FAILED!");
                    return response;
                }
                response = Encoding.UTF8.GetBytes("OK");
                return response;
            }
            else
            {
                response = Encoding.UTF8.GetBytes("FAILED!");
                return response;
            }
        }

        //FUNÇÃO PARA CONFIRMAR O REGISTO
        private static bool userConfirmRegister(string user,string email)
        {
            serverContext context = new serverContext();
           
            //ENTITY FRAMEWORK
            var query_result_alt = context.Users.Where(
             User=>
             User.user == user &&
             User.email == email);

            if (query_result_alt.Count() == 0)
            {
                Console.WriteLine("Register Successful");
                return true;
            }
            Console.WriteLine("Register Failed");
            return false;

        }
        //FUNÇÃO PARA TRATAR O LOGIN
        private static byte[] handleLogin(string[] _data)
        {
            byte[] response = null;
            string user = _data[1];
            string pass = _data[2];

            if (authUser(user, pass))
            {
                Console.WriteLine("Login Successful!");
                response = Encoding.UTF8.GetBytes("OK");
            }
            else
            {
                Console.WriteLine("Login Failed!");
                response = Encoding.UTF8.GetBytes("FAILED!");
            }
            return response;
        }
        //FUNÇÃO PARA TRATAR A MENSAGEM
        private static string handleMessage(string[] _data)
        {
            Console.WriteLine("handle message -> " + _data[0] + _data[1] + _data[2]);
            //byte[] response = Encoding.UTF8.GetBytes(_data[0] + "-" + _data[1] + "-" + _data[2] + "-" + _data[3]);
            string response = _data[0] + "-" + _data[1] + "-" + _data[2] + "-" + _data[3];
            return response;
        }

        //FUNÇÃO PARA MANDAR A MENSAGEM PARA TODOS OS CLIENTES
        private static void BroadcastMessage(string message, string userI, string userO = null)
        {
            if (userI == "Scream")
            {
                foreach (Client client in Clients)
                {
                    byte[] sendMessage = Encoding.UTF8.GetBytes(encryptText(message, client.PublicKey));

                    NetworkStream stream = client.TcpClient.GetStream();
                    stream.Write(sendMessage, 0, message.Length);
                }
            }
            else
            {
                foreach (Client client in Clients)
                {
                    if (userI == client.User || userO == client.User)
                    {
                        byte[] sendMessage = Encoding.UTF8.GetBytes(encryptText(message, client.PublicKey));
                        NetworkStream stream = client.TcpClient.GetStream();
                        stream.Write(sendMessage, 0, message.Length);
                    }
                }
            }
            
        }

        private static string handleListUsers()
        {
            string _user;
            _user = "UserList";
            foreach (Client client in Clients)
            {
                 _user += "-" + client.User;
            }
            Console.WriteLine(_user);

            return _user;
        }

        public static string encryptText(string text, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                Console.WriteLine(text);
                byte[] plaintextBytes = Encoding.UTF8.GetBytes(text);
                byte[] cypheredText = rsa.Encrypt(plaintextBytes, false);
                Console.WriteLine(cypheredText);
                return Convert.ToBase64String(cypheredText);
            }

        }

        public static string decryptText(string text, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);
                Console.WriteLine("decrypt text ->"+text);
                byte[] cryptedText = Convert.FromBase64String(text); 
                cryptedText = rsa.Decrypt(cryptedText, false);
                string plainText = Encoding.UTF8.GetString(cryptedText);
                Console.WriteLine(plainText);
                return plainText;
            }
        }

        private static void createLog(string command, string user, string target = null)
        {
            //string path = "log.txt"; //Caminho do ficheiro de log
            string path = @"../../log.txt";
            try
            {
                if (!File.Exists(path)) //Verificar se o ficheiro existe
                {
                    File.Create(path);
                }
                
                //Abrir o ficheiro para escrita
                using (StreamWriter logWriter = File.AppendText(path))
                {
                    logWriter.WriteLine(DateTime.Now + " -> " + command + " - " + user + " - " + target);
                    logWriter.Close();
                }
            }
            catch (Exception ex)
            {
                //Error Handler
                using (StreamWriter logWriter = File.AppendText("log.txt"))
                {
                    logWriter.WriteLine("Error: " + ex);
                }
            }
        }
    }
}