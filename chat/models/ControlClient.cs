using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.models
{
    internal class ControlClient
    {
        public string PublicKey { get; set; }
        public bool State { get; set; }

        public string User {  get; set; }

        public ControlClient(string user, bool state) // To Do ADD the Public key
        {
            state = false;
            User = user;
        }
    }
}
