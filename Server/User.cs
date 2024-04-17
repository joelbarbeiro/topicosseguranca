using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class User
    {
        public String user {  get; set; }

        public String password { get; set; }

        public User(string user, string password)
        {
            this.user = user;
            this.password = password;
        }

        public override string ToString()
        {
            return user + " " + password;
        }
    }
}
