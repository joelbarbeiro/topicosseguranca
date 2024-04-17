using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.models
{
    internal class MessageChat
    {
        public string User { get; set; }
        public DateTime Data { get; set; }
        public string Messag { get; set; }

        public MessageChat(string messag, string user)
        {
            User = user;
            Messag = messag;
            Data = DateTime.Now;
        }


        public override string ToString()
        {
            return Data.ToString("HH:mm:ss") + " - " + User + " - " + Messag;
        }
    }

}
