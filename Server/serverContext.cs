using Server.models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class serverContext : DbContext
    {
        public DbSet<MsgHistory> msgsHistory { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
