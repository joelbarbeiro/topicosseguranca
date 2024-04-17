using chat.models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace chat
{
    internal class clientContext:DbContext
    {
        public DbSet<MessageChat> messages { get; set; }
        public DbSet<ControlClient> controls { get; set; }
    }
}
