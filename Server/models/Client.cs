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

        public Client(TcpClient tcpClient,string user)
        {
            this.user = user;
            this.tcpClient = tcpClient;
           
        }
        public override string ToString()
        {
            return user;
        }
    }
}
