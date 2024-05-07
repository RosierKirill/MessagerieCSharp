using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestServeur
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!serveur1.IsRunning)
            {
                if (this.serveur1.Start(null, 1234))
                    this.startButton.Enabled = false;
            }

        }

        private void serveur1_NetworkClientClose(object sender, NetworkTools.Controls.NetworkServerEventArgs e)
        {
            this.listBoxEvents.Items.Add(">> " + e.Client.Id.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serveur1.IsRunning)
                this.serveur1.Stop();
        }

        private void serveur1_NetworkClientAccept(object sender, NetworkTools.Controls.NetworkServerEventArgs e)
        {
            this.listBoxEvents.Items.Add("<< " + e.Client.Id.ToString());
        }
    }
}
