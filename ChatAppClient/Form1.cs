using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ChatAppClient
{
    public partial class Form1 : Form
    {
        public static string ipField { get; private set; }

        public static string usernameField { get; private set; }

        public static Form1 Instance => instance;

        private static Form1 instance;

        public Form1()
        {
            InitializeComponent();

            if (instance == null)
            {
                instance = this;
            }
        }

        private void IP_TextChanged(object sender, EventArgs e)
        {
            ipField = IP.Text;
        }

        private void Username_TextChanged(object sender, EventArgs e)
        {
            usernameField = Username.Text;
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            Client.ConnectToServer();
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            Client.Disconnect();
        }

        public void Connected()
        {
            Connect.Enabled = false;
            IP.Enabled = false;
            Username.Enabled = false;
            Disconnect.Enabled = true;
        }

        public void Disconnected()
        {
            Connect.Enabled = true;
            IP.Enabled = true;
            Username.Enabled = true;
            Disconnect.Enabled = false;
        }
    }
}
