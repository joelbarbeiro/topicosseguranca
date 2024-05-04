using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public String user { get; set; }
        public String password { get; set; }
        public String email { get; set; }
        public bool isLoggedIn { get; set; }

        public User()
        {
        }

        public User(string user, string password, string email)
        {
            this.user = user;
            this.password = password;
            this.isLoggedIn = false;
            this.email = email;

        }

        public override string ToString()
        {
            return user +" "+email+" "+ password;
        }
    }
}
