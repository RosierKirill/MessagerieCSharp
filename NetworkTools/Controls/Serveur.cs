using NetworkTools.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkTools.Controls
{
    public class NetworkServerEventArgs : EventArgs
    {
        public ServeurReseau Serveur;
        public ClientReseauServeur Client;
    }


    public class Serveur : UserControl
    {
        private ServeurReseau server;

        public event EventHandler<NetworkServerEventArgs> NetworkClientAccept;
        public event EventHandler<NetworkServerEventArgs> NetworkClientClose;
        public Serveur()
        {
            NetworkClientAccept = null;
            NetworkClientClose = null;
            base.Visible= false;
        }

        // Pour cacher l'accès à la propriété Visible
        new bool Visible { get; set; }

        /// <summary>
        /// Démarrage du Serveur
        /// </summary>
        /// <param name="adresseIP">Adresse à écouter, Null ou Vide pour toutes les adresses</param>
        /// <param name="port">Port à écouter</param>
        /// <returns>Vrai si le Serveur a démarré</returns>
        /// <exception cref="Exception">Déclenchée si un Server fonctionne déjà</exception>
        public bool Start(string adresseIP, int port)
        {
            if (server == null)
            {
                server = new ServeurReseau(adresseIP, port);
                server.Start();
                if (server.IsRunning)
                {
                    server.OnClientAccept += OnClientAccept;
                    server.OnClientClose += OnClientClose;
                    return true;
                }
                else
                {
                    server.Stop();
                    return false;
                }
            }
            else
                throw new Exception("Server is already running");
        }

        private void OnClientClose(ServeurReseau server, ClientReseauServeur clt)
        {
            if (this.NetworkClientClose != null)
            {
                try
                {
                    // !!! ATTENTION !!!
                    // A ce stade on est "encore" dans le contexte d'execution du Thread Réseau
                    NetworkServerEventArgs args = new NetworkServerEventArgs();
                    args.Client = clt;
                    args.Serveur = server;
                    this.Invoke(this.NetworkClientClose, new object[] { this, args });
                }
                catch
                {

                }
            }
                
        }

        private void OnClientAccept(ServeurReseau sender, ClientReseauServeur clt)
        {
            if (this.NetworkClientAccept != null)
            {
                // !!! ATTENTION !!!
                // A ce stade on est "encore" dans le contexte d'execution du Thread Réseau
                NetworkServerEventArgs args = new NetworkServerEventArgs();
                args.Client = clt;
                args.Serveur = server;
                this.Invoke(this.NetworkClientAccept, new object[] { this, args });
            }
        }

        /// <summary>
        /// Arrêt du Serveur
        /// </summary>
        /// <returns>Arrêt du Serveur</returns>
        /// <exception cref="Exception">Déclenchée si aucun Server ne fonctionne</exception>
        public bool Stop()
        {
            if (server != null)
            {
                server.Stop();
                server = null;
                return true;
            }
            else
                throw new Exception("No Server is running");
        }

        /// <summary>
        /// Indique si un Serveur est en cours de fonctionnement.
        /// </summary>
        public bool IsRunning
        {
            get { return (server != null) && (server.IsRunning); }
        }





    }
}
