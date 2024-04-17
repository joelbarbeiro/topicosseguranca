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

namespace Server
{
    internal class Program
    {
        private static List<User> Users;
        private const int Port = 10000;
        static void Main(string[] args)
        {
            Users = new List<User>();


            //ESCREVER PARA CONSOLA
            Console.WriteLine("A iniciar o servidor...");

            // le ficheiuro user para autenticacao
            readFileUsers();

            //CICLO INFINITO PARA ESTAR SEMPRE À ESCUTA
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
            TcpListener listener = new TcpListener(endPoint);
            listener.Start();
            Console.WriteLine("SERVER READY!");
            int clientCounter = 0;
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clientCounter++;
                Console.WriteLine("Client {0} connected", clientCounter);
                ClientHandler clientHandler = new ClientHandler(client, clientCounter);
                clientHandler.Handle();
            }
        }
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
                NetworkStream networkStream = this.client.GetStream();
                ProtocolSI protocolSI = new ProtocolSI();
                while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] ack;
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            string[] type = protocolSI.GetStringFromData().Split('-');
                            Console.WriteLine("Client" + clientID + ": " + protocolSI.GetStringFromData());
                            ack = typedMessage(type);
                            //ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("Ending Thread from Client" + "{0}", clientID);
                            protocolSI.GetStringFromData();
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;
                    }
                }
                networkStream.Close();
                client.Close();
            }
        }

        private static byte[] typedMessage(string[] type)
        {
            byte[] response = null;
            switch (type[0])
            {
                case "Login":
                    response=handleLogin(type); 
                    break;

                case "Message":
                    response=handleMessage(type);
                    break;
            }
            return response;
        }

        private static void readFileUsers()
        {
            string path = @"../../userList.txt";
            try
            {
                //query

                string line;

                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader r = new StreamReader(fs, Encoding.UTF8);

                while ((line = r.ReadLine()) != null)
                {
                    string[] userData = line.Split(';');

                    if (userData.Length == 2)
                    {
                        User user = new User(userData[0], userData[1].Trim());
                        Users.Add(user);
                    }
                }
                r.Close();
                fs.Close();

                foreach (User user in Users) {
                 Console.WriteLine(user.ToString());
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Erro "+ex);
            }
        }
        private static bool authUser(string[] _data)
        {
            
            foreach(User user in Users)
            {
                if (user.user == _data[1] && user.password == _data[2])
                {
                    user.isLoggedIn = true;
                    return true;
                }
            }
            return false;
        }
        private static bool checkLoggedIn(string[] _data)
        {
            foreach(User tmp in Users)
            {
                if(tmp.user == _data[1])
                {
                    return tmp.isLoggedIn;
                }
            }
            return false;
        }
        private static byte[] handleLogin(string[] _data)
        {
            byte[] response = null;
            if (authUser(_data))
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
        private static byte[] handleMessage(string[] _data)
        {

            return null;
        }
    }
}
