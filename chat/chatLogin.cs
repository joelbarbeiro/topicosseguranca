﻿using System;
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
using System.Security.Cryptography.X509Certificates;
using chat;
using EI.SI;
using chat.models;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace chat
{
    public partial class FormChatLogin : Form
    {

        private const int Port = 10000;
        public static NetworkStream netStream;
        TcpClient tcpClient;
        ProtocolSI protocolSI;
        public string user;
        public string pubKey;
        public string privKey;
        public string serverPubKey;


        public FormChatLogin(TcpClient tcpClient, string privKey, string pubKey, string serverPubKey)
        {
            this.tcpClient = tcpClient;
            this.privKey = privKey;
            this.pubKey = pubKey;
            this.serverPubKey = serverPubKey;
            InitializeComponent();
            protocolSI = new ProtocolSI();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string response;
            int size;
            string msg = CryptFunctions.AESEncrypt(pubKey);
            byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, msg);
            netStream.Write(packet, 0, packet.Length);
            size = netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size);
            string plainText = CryptFunctions.AESDecrypt(response);
            string[] getServerStat = plainText.Split('-');

            if (getServerStat[0] == "Ok")
            {
                serverPubKey = getServerStat[1];
                Console.WriteLine(serverPubKey);
                plainText = "Login-" + textBoxLoginUsername.Text + "-" + CryptFunctions.genPassHash(textBoxLoginPassword.Text);
                msg = CryptFunctions.encryptText(plainText, serverPubKey);
                Console.WriteLine(msg);
                packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, msg);
                netStream.Write(packet, 0, packet.Length);
                size = netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                response = Encoding.UTF8.GetString(protocolSI.Buffer, 0, size);
                plainText = CryptFunctions.decryptText(response, privKey);

                changeForm(plainText);
            }
        }

        private void textBoxLoginUsername_Enter(object sender, EventArgs e)
        {
            textBoxLoginUsername.Text = "";
            loginButtonControl();
        }
        private void textBoxLoginUsername_Leave(object sender, EventArgs e)
        {
            if(textBoxLoginUsername.Text.Length == 0)
            {
                textBoxLoginUsername.Text = "Username:";
            }
            loginButtonControl();
        }
        private void textBoxLoginPassword_Enter(object sender, EventArgs e)
        {
            textBoxLoginPassword.Text = "";
            loginButtonControl();
        }
        private void textBoxLoginPassword_Leave(object sender, EventArgs e)
        {
            if (textBoxLoginPassword.Text.Length == 0)
            {
                textBoxLoginPassword.Text = "Password:";
            }
            loginButtonControl();
        }

        private void loginButtonControl()
        {
            if ((textBoxLoginUsername.Text.Length == 0 || textBoxLoginUsername.Text != "Username:") && (textBoxLoginPassword.Text.Length == 0 || textBoxLoginPassword.Text != "Password:"))
            {
                buttonLogin.Enabled = true;
            }
            else
            {
                buttonLogin.Enabled = false;
            }
        }

        private void changeForm(string response)
        {
            if (response == "OK")
            {
                user = textBoxLoginUsername.Text;
                FormChat mainForm = new FormChat(user,tcpClient, netStream, privKey, pubKey, serverPubKey);
                FormChat.ReceiveNetworkStream(netStream);
                mainForm.Show();
                this.Hide();
            }
            else
            { 
                MessageBox.Show("Login failed. Please try again.");
            }
        }

        private void buttonChatRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            formChatRegister chatRegisterForm = new formChatRegister(tcpClient, netStream, privKey, pubKey, serverPubKey);       
            chatRegisterForm.ShowDialog();
        }

        private void CloseClient()
        {
            byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
            netStream.Write(eot, 0, eot.Length);
            netStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            netStream.Close();
            tcpClient.Close();
        }

        private void FormChatLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseClient();
        }

        public class TcpClientSingleton
        {
            private static TcpClient instance;
            private static readonly object lockObject = new object();

            private TcpClientSingleton() { }

            public static TcpClient GetInstance()
            {
                lock (lockObject)
                {
                    if (instance == null || !instance.Connected)
                    {
                        // Create a new TCP client if it's null or not connected
                        instance = new TcpClient();
                    }
                    return instance;
                }
            }
        }

        public static void ReceiveNetworkStream(NetworkStream Stream)
        {
            netStream = Stream;
        }
    }
}
