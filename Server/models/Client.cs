using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public TcpClient tcpClient { get; set; }
        public string user { get; set; }
        public string publicKey { get; set; }

        public Client()
        {
        }

        public Client(int client, TcpClient tcpClient, string user)
        {
            this.tcpClient = tcpClient;
            this.user = user;
        }
        public override string ToString()
        {
            return user;
        }
    }
}
