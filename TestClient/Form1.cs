using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            this.client1.Send( this.textBoxMessage.Text );
        }

        private void connectionButton_Click(object sender, EventArgs e)
        {
            if (!client1.IsRunning)
            {
                if (this.client1.Connect("127.0.0.1", 1234))
                    this.connectionButton.Enabled = false;

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ( client1.IsRunning)
            {
                this.client1.Close();
            }
        }

        private void client1_NetworkClientMessageReceived(object sender, NetworkTools.Controls.NetworkClientEventArgs e)
        {
            this.textBoxRecu.Text = e.Message.Data;
        }
    }
}
