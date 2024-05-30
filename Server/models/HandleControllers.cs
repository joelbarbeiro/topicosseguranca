using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Server.models
{
    public class HandleControllers
    {

        //FUNÇÃO PARA TRATAR DO REGISTO DAS CONTAS
        public static string handleRegister(string[] _data)
        {
            string response = null;
            string user = _data[1];
            string pass = _data[2];
            string email = _data[3];

            if (ValidationControllers.userConfirmRegister(user, email))
            {
                User new_account = new User(user, pass, email);
                serverContext context = new serverContext();
                context.Users.Add(new_account);

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Couldn't save new account   -- " + ex);
                    response = "FAILED!";
                    return response;
                }
                response = "OK";
                return response;
            }
            else
            {
                response = "FAILED!";
                return response;
            }
        }
        //FUNÇÃO PARA TRATAR O LOGIN
        public static string handleLogin(List<string> _data)
        {
            string response = null;
            string user = _data[1];
            string pass = _data[2];

            if (ValidationControllers.authUser(user, pass))
            {
                Console.WriteLine("Login Successful!");
                response = "OK";
            }
            else
            {
                Console.WriteLine("Login Failed!");
                response = "FAILED!";
            }
            return response;
        }
        //FUNÇÃO PARA TRATAR A MENSAGEM
        /*public static string handleMessage(List<string> _data)
        {
            Console.Write("handle message -> ");
            foreach (string _item in _data)
            {
                Console.Write(" " + _item);
            }

            //byte[] response = Encoding.UTF8.GetBytes(_data[0] + "-" + _data[1] + "-" + _data[2] + "-" + _data[3]);
            string response = _data[0] + "ᚼ" + _data[1] + "ᚼ" + _data[2] + "ᚼ" + _data[3];
            
            return response;
        }*/

        public static List<string> handleListUsers()
        {
            List<string> users= new List<string>();
            
            users.Add("UserList") ;
            users.Add("Scream");
            foreach (Client client in Program.Clients)
            {
                users.Add(client.User);
            }
            return users;
        }
        public static string getKeyFromList(TcpClient client)
        {
            foreach(Client user in Program.Clients)
            {
                if(user.TcpClient == client)
                {
                    Console.WriteLine("key found -> "+ user.PublicKey);
                    return user.PublicKey;
                }
            }
            return null;
        }
        public static void removeUserFromList(TcpClient client)
        {
            Program.Clients.RemoveAll(user => user.TcpClient == client);
        }
    }
}
