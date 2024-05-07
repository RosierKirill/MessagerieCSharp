using NetworkTools.Client;
using NetworkTools.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkTools.Controls
{
    public class NetworkClientEventArgs : EventArgs
    {
        public MessageManagement Client;
        public Message Message;
    }


    /// <summary>
    /// Un Control à jeter sur la fenetre pour gerer le client de messagerie
    /// </summary>
    public class Client : UserControl
    {
        private MessageManagement client;

        public event EventHandler<NetworkClientEventArgs> NetworkClientMessageReceived;
        public event EventHandler<NetworkClientEventArgs> NetworkClientClientDisconnected;


        /// <summary>
        /// Création d'une Client
        /// </summary>
        public Client()
        {
            NetworkClientMessageReceived = null;
            NetworkClientClientDisconnected = null;
            base.Visible = false;
        }

        // Pour cacher l'accès à la propriété Visible
        new bool Visible { get; set; }


        /// <summary>
        /// Connection au Serveur distant
        /// </summary>
        /// <param name="adresseIP">Adresse IP V4 du serveur</param>
        /// <param name="port">Port du service</param>
        /// <returns>Indique si la connection a réussie</returns>
        /// <exception cref="Exception">Déclenchée si un Client fonctionne déjà</exception>
        public bool Connect(String adresseIP, int port)
        {
            if (client == null)
            {
                this.client = new MessageManagement(adresseIP, port);
                this.client.Start();
                if (this.client.Connected)
                {
                    this.client.OnMessageReceived += OnMessageReceived;
                    this.client.OnClientDisconnected += OnClientDisconnected;
                    return true;
                }
                else
                {
                    this.client.Stop();
                    this.client = null;
                    return false;
                }
            }
            else
                throw new Exception("Client is already existing");
        }

        /// <summary>
        /// Arrêt de la Connection
        /// </summary>
        /// <returns>Arrêt de la Connection</returns>
        /// <exception cref="Exception">Déclenchée si aucun Client ne fonctionne</exception>
        public bool Close()
        {
            if (client != null)
            {
                client.Stop();
                client = null;
                return true;
            }
            else
                throw new Exception("No Client is running");
        }

        public void Send(string msg)
        {
            if (this.client != null)
            {
                this.client.Send(msg);
            }
        }

        /// <summary>
        /// Indique si un Client est en cours de fonctionnement.
        /// </summary>
        public bool IsRunning
        {
            get { return (client != null) && (client.IsRunning); }
        }

        private void OnClientDisconnected(MessageManagement sender)
        {
            if (this.NetworkClientClientDisconnected != null)
            {
                // !!! ATTENTION !!!
                // A ce stade on est "encore" dans le contexte d'execution du Thread Réseau
                NetworkClientEventArgs args = new NetworkClientEventArgs();
                args.Client = this.client;
                args.Message = null;
                this.Invoke(this.NetworkClientClientDisconnected, new object[] { this, args });
            }
        }

        private void OnMessageReceived(MessageManagement sender, Message message)
        {
            if (this.NetworkClientMessageReceived != null)
            {
                // !!! ATTENTION !!!
                // A ce stade on est "encore" dans le contexte d'execution du Thread Réseau
                NetworkClientEventArgs args = new NetworkClientEventArgs();
                args.Client = this.client;
                args.Message = message;
                this.Invoke(this.NetworkClientMessageReceived, new object[] { this, args });
            }
        }
    }
}
