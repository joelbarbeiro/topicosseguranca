using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat
{
    internal class Control
    {
        public string publicKey {  get; set; }
        public string state { get; set; }

        public Control(string publicKey, string state)
        {
            this.publicKey = publicKey;
            this.state = state;
        }
    }
}
