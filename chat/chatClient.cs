using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using chat.models;
using EI.SI;

namespace chat
{
    public partial class FormChat : Form
    {

        private List<MessageChat> Messages;
        private const int Port = 10000;
        NetworkStream netStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;

        public FormChat()
        {
            InitializeComponent();
            Messages = new List<MessageChat>();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, Port);
            tcpClient = new TcpClient();
            tcpClient.Connect(endPoint);
            netStream = tcpClient.GetStream();
            protocolSI = new ProtocolSI();

        }


        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            string msg = textBoxMessage.Text;
            textBoxMessage.Clear();
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
            netStream.Write(packet, 0, packet.Length);
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }

        }

        private void CloseClient()
        {
            byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
            netStream.Write(eot, 0, eot.Length);
            netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            netStream.Close();
            tcpClient.Close();
        }


        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseClient();
        }
    }
}
