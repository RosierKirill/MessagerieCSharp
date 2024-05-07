using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using NetworkTools;

namespace NetworkTools.Server
{
    public class ClientReseauServeur
    {
        private TcpClient tcpClient;
        private Thread threadLecture;
        private NetworkStream clientStream;
        private bool running;

        internal Mutex AccessMessages { get; set; }
        internal AutoResetEvent SignalementMessage { get; set; }

        /// <summary>
        /// Retourne une chaine qui correspond à l'adresse IP du ClientReseau et le port associé
        /// </summary>
        public String Id
        {
            get
            {
                try
                {
                    IPEndPoint ep = (IPEndPoint)this.tcpClient.Client.RemoteEndPoint;
                    return ep.Address.GetHashCode().ToString() + "_" + Port.ToString();
                }
                catch { }
                //
                return null;
            }
        }
        internal Stack<Message> Messages { get; set; }
        internal ServeurReseau Serveur { get; set; }

        /// <summary>
        /// L'adresse IP du Client, sous forme de chaine de caractères
        /// </summary>
        public string IPAddress
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)this.tcpClient.Client.RemoteEndPoint;
                String ip = ep.ToString();
                return ip;
            }
        }

        /// <summary>
        /// Le port de connection Client
        /// </summary>
        public int Port
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)this.tcpClient.Client.RemoteEndPoint;
                return ep.Port;
            }
        }

        /// <summary>
        /// Crée un ClientReseau à partir d'un TcpClient
        /// </summary>
        /// <param name="client"></param>
        internal ClientReseauServeur(TcpClient client)
        {
            this.tcpClient = client;
            this.running = false;
        }

        public void Start()
        {
            //
            ThreadStart PointEntree = new ThreadStart(this.Lire);
            threadLecture = new Thread(PointEntree);
            threadLecture.Start();
            //
        }

        public void Stop()
        {
            if (running)
            {
                this.clientStream.Close();
            }
        }


        private void Lire()
        {
            this.running = true;
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
                // Mais on met l'Id du client coté Server
                Message msg = new Message(this.Id, reception);
                // On stocke
                this.Messages.Push(msg);
                // On libère l'accès
                this.AccessMessages.ReleaseMutex();

                // Il faut signaler au Serveur qu'on a reçu un message
                // donc on leve le drapeau
                this.SignalementMessage.Set();
            }
            //
            this.running = false;
            this.Serveur.RemoveClient(this.Id);
        }

        public void Send(String msg)
        {
            if (this.running)
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(msg);
                this.clientStream.Write(buffer, 0, buffer.Length);
            }
        }


    }
}
