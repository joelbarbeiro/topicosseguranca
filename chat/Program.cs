using EI.SI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static chat.FormChatLogin;

namespace chat
{

    internal static class Program
    {
        private const int Port = 10000;
        private static NetworkStream netStream;
        private static TcpClient tcpClient;
        private static ProtocolSI protocolSI;
        private static string pubKey;
        private static string privKey;
        private static string serverPubKey = string.Empty;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new FormChatLogin());
        }

        

    }
}
