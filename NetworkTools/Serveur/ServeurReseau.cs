using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using NetworkTools;

namespace NetworkTools.Server
{
    public delegate void ClientEvent(ServeurReseau sender, ClientReseauServeur clt);

    /// <summary>
    /// Serveur Réseau
    /// </summary>
    public class ServeurReseau
    {
        private Thread threadEcoute;
        private Thread threadDispatch;
        private Mutex accessMessages;
        private Mutex accessClients;
        private AutoResetEvent signalementMessage;
        private AutoResetEvent demandeSortie;

        private bool running;
        private IPEndPoint ep;
        private TcpListener ecoute;

        private List<ClientReseauServeur> clients;
        private Stack<Message> messages;

        public ClientEvent OnClientAccept;
        public ClientEvent OnClientClose;

        /// <summary>
        /// Creation d'un serveur réseau
        /// </summary>
        /// <param name="adresseIP">Adresse à écouter, Null ou Vide pour toutes les adresses</param>
        /// <param name="port">Port à écouter</param>
        public ServeurReseau(String adresseIP, int port)
        {
            if (!String.IsNullOrEmpty(adresseIP))
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
            else
            {
                this.ep = new IPEndPoint(IPAddress.Any, port);
            }
            ecoute = new TcpListener(this.ep);
            running = false;
            this.accessMessages = new Mutex();
            this.accessClients = new Mutex();
            this.signalementMessage = new AutoResetEvent(false);
            this.demandeSortie = new AutoResetEvent(false);
        }

        /// <summary>
        /// Démarrage du Server Réseau
        /// </summary>
        public void Start()
        {
            // On met l'écoute dans un Thread
            ThreadStart PointEntree = new ThreadStart(this.Ecoute);
            threadEcoute = new Thread(PointEntree);
            threadEcoute.Start();
            // et le dispatch dans un autre
            PointEntree = new ThreadStart(this.Dispatch);
            threadDispatch = new Thread(PointEntree);
            threadDispatch.Start();
            //
            this.running = true;
        }

        public bool IsRunning{ get => running; }

        private void Dispatch()
        {
            WaitHandle[] attente = new WaitHandle[2];
            //
            attente[0] = this.signalementMessage;
            attente[1] = this.demandeSortie;
            while (true)
            {
                // On attend, soit le Signalement de Message, soit le Demande de Fermeture du Serveur
                int who = AutoResetEvent.WaitAny(attente, Timeout.Infinite);
                if (who == 1)
                {
                    this.accessClients.WaitOne();
                    // Sortie Demandée ! Ciao
                    foreach (ClientReseauServeur clt in this.clients)
                    {
                        // On ferme les clients réseaux
                        clt.Stop();
                    }
                    this.accessClients.ReleaseMutex();
                    break;
                }
                // Il y a un message en attente de Dispatch
                // On accède à la pile des messages
                this.accessMessages.WaitOne();
                // On lit le message en pile
                Message msg = null;
                while (this.messages.Count > 0)
                {
                    msg = this.messages.Pop();
                    //
                    this.accessClients.WaitOne();
                    foreach (ClientReseauServeur clt in this.clients)
                    {
                        if (clt.Id != msg.Id)
                        {
                            // On l'envoie à tous les autres
                            clt.Send(msg.Data);
                        }
                    }
                    this.accessClients.ReleaseMutex();
                }
                // On libère
                this.accessMessages.ReleaseMutex();
            }
            //
        }

        private void Ecoute()
        {
            this.ecoute.Start();
            //
            this.clients = new List<ClientReseauServeur>();
            this.messages = new Stack<Message>();
            
            //
            while (true)
            {
                try
                {
                    // Récupération du Client
                    TcpClient client = this.ecoute.AcceptTcpClient();
                    // Lancement d'un Thread avec ce Client
                    ClientReseauServeur clientNetwork = new ClientReseauServeur(client);
                    clientNetwork.AccessMessages = this.accessMessages;
                    clientNetwork.Messages = this.messages;
                    clientNetwork.SignalementMessage = this.signalementMessage;
                    clientNetwork.Serveur = this;
                    clientNetwork.Start();
                    //
                    this.accessClients.WaitOne();
                    clients.Add(clientNetwork);
                    this.accessClients.ReleaseMutex();
                    // En cas d'erreur de traitement du sous-traitant
                    // On remonte l'erreur
                    if (this.OnClientAccept != null)
                    {
                        this.OnClientAccept(this, clientNetwork);
                    }
                    
                }
                catch (InvalidOperationException e)
                {
                    // On remonte l'erreur
                    throw e;
                }
                catch (Exception e)
                {
                    // Mmm pas bon, peut etre une fermeture du TcpListener ??
                    // On pouse les infos dans la fenêtre du DEBUG si il y en a un...
                    Trace.WriteLine(e.Message);
                    //et on s'en va
                    break;
                }
            }
            this.running = false;
        }

        /// <summary>
        /// Arret du Serveur
        /// </summary>
        public void Stop()
        {
            if (this.running)
            {
                // Il faut arrêter le Thread
                // On va donc fermer le listener
                this.ecoute.Stop();
                // Et on demande la sortie du Dispatch
                this.demandeSortie.Set();
            }
        }


        internal void RemoveClient(string id)
        {
            this.accessClients.WaitOne();
            //
            ClientReseauServeur clt = this.clients.Find(x => x.Id == id);
            if (clt != null)
                clients.Remove(clt);
            this.accessClients.ReleaseMutex();
            //
            if (clt != null)
            {
                try
                {
                    if (this.OnClientClose != null)
                        this.OnClientClose(this, clt);
                }
                catch (InvalidOperationException e)
                {
                    // On remonte l'erreur
                    throw e;
                }
            }
        }

    }
}
