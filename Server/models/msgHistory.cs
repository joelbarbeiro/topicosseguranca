using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.models
{
    public class msgHistory
    {
        [Key]
        public int idMsgHist { get; set; }
        public string User { get; set; }
        public DateTime dateTime { get; set; }
        public string Message { get; set; }

        public msgHistory()
        {
        }

        public msgHistory(string user, DateTime dateTime, string message)
        {
            User = user;
            this.dateTime = dateTime;
            Message = message;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
