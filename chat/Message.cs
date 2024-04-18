using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    internal class Message
    {
        public string User { get; set; }
        public DateTime Data { get; set; }
        public string Messag { get; set; }

        public Message(string messag)
        {
            Messag = messag;
        }
        public override string ToString()
        {
            return Messag;
        }
    }

}
