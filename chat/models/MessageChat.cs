using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.models
{
    internal class MessageChat
    {
        [Key]
        public int MsgId { get; set; }
        public string User { get; set; }
        public DateTime Data { get; set; }
        public string Messag { get; set; }

        public MessageChat()
        {
        }

        public MessageChat(string user, string messag)
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
