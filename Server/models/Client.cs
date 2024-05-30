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
        public TcpClient TcpClient { get; set; }
        public string User { get; set; }
        public string PublicKey { get; set; }

        public Client()
        {
        }

        public Client(TcpClient tcpClient, string user, string publicKey)
        {
            this.User = user;
            this.TcpClient = tcpClient;
            this.PublicKey = publicKey;
        }
        public override string ToString()
        {
            return User;
        }
    }
}
