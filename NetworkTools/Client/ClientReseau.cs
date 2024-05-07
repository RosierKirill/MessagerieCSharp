using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkTools;

namespace NetworkTools.Client
{
    /// <summary>
    /// ClientReseau gère la connection/deconnexion, la lecture, l'envoi de message (chaine de caractères)
    /// Cette classe est utilisée par MessageManagement
    /// </summary>
    internal class ClientReseau
    {
        private TcpClient tcpClient;
        private Thread threadLecture;
        private NetworkStream clientStream;
        private IPEndPoint ep;


        internal Mutex AccessMessages { get; set; }
        internal AutoResetEvent SignalementMessage { get; set; }
        internal Stack<Message> Messages { get; set; }
        internal bool Connected { get; private set; }

        internal bool IsRunning { get; private set; }
        internal AutoResetEvent SignalementSortie { get; set; }

        /// <summary>
        /// Retourne un String qui correspond à l'adresse IP du ClientReseau et au Port associé
        /// </summary>
        public String Id
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)this.tcpClient.Client.LocalEndPoint;
                return ep.Address.GetHashCode().ToString() + "_" + Port.ToString();
            }
        }

        /// <summary>
        /// L'adresse IP locale du Client, sous forme de chaine de caractères
        /// </summary>
        public string IPAddress
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)this.tcpClient.Client.LocalEndPoint;
                String ip = ep.ToString();
                return ip;
            }
        }

        /// <summary>
        /// Le port de connection Client local
        /// </summary>
        public int Port
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)this.tcpClient.Client.LocalEndPoint;
                return ep.Port;
            }
        }

        internal ClientReseau()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// Crée un Client réseau
        /// </summary>
        /// <param name="adresseIP">Adresse IP V4 du Serveur</param>
        /// <param name="port">Port du service</param>
        /// <exception cref="Exception"></exception>
        public ClientReseau(String adresseIP, int port) : this()
        {
            String[] elements = adresseIP.Split('.');
            if (elements.Length != 4)
                throw new Exception("Il faut 4 éléments dans une adresse IP");
            //
            byte[] elts = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                String element = elements[i];
                byte elt = Convert.ToByte(element);
                elts[i] = elt;
            }
            //
            IPAddress ipaddd = new IPAddress(elts);
            //
            this.ep = new IPEndPoint(ipaddd, port);
        }

        /// <summary>
        /// Connection au Server
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(this.ep);
                this.Connected = tcpClient.Connected;
                if (this.Connected)
                {
                    this.clientStream = tcpClient.GetStream();
                }
            }
            catch
            {
                this.Connected = false;
            }
            return this.Connected;
        }

        /// <summary>
        /// Déconnection
        /// </summary>
        public void Disconnect()
        {
            if (Connected)
            {
                // Fermer le flux, ferme la connection
                this.clientStream.Close();
            }
        }

        /// <summary>
        /// Démarrage du Thread d'échange
        /// </summary>
        public void Start()
        {
            //
            ThreadStart PointEntree = new ThreadStart(this.Lire);
            threadLecture = new Thread(PointEntree);
            threadLecture.Start();
            //
        }


        private void Lire()
        {
            this.IsRunning = true;
            // On récupère le flux avec les infos
            this.clientStream = tcpClient.GetStream();
            //
            byte[] message = new byte[4096];
            int bytesRead;
            while (true)
            {
                bytesRead = 0;
                try
                {
                    //
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    // socket error, on sort
                    break;
                }
                if (bytesRead == 0)
                {
                    // disconnected, on sort
                    break;
                }
                //message reçu
                ASCIIEncoding encoder = new ASCIIEncoding();
                String reception = encoder.GetString(message, 0, bytesRead);
                // On met le message en attente
                // On demande un accès aux Messages
                this.AccessMessages.WaitOne();
                // On reconstitue le message à partir des données brutes
                // On stocke
                this.Messages.Push(new Message( this.Id, reception));
                // On libère l'accès
                this.AccessMessages.ReleaseMutex();

                // Il faut signaler au Serveur qu'on a reçu un message
                // donc on leve le drapeau
                this.SignalementMessage.Set();
            }
            //
            this.IsRunning = false;
            this.SignalementSortie.Set();
        }

        public void Ecrire(String msg)
        {
            if (this.IsRunning)
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(msg);
                this.clientStream.Write(buffer, 0, buffer.Length);
            }
        }

    }
}
