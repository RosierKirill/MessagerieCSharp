using NetworkTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkTools.Client
{
    public delegate void MessageReceived(MessageManagement sender, Message message);
    public delegate void ClientDisconnected(MessageManagement sender);

    /// <summary>
    /// Message%anagement définit une classe de gestion des messages reçus.
    /// Elle est utilisée dans le Client de messagerie
    /// </summary>
    public class MessageManagement
    {
        private string address;
        private int port;
        private Stack<Message> messages;
        Thread thEcouter;
        ClientReseau client;

        public MessageReceived OnMessageReceived;
        public ClientDisconnected OnClientDisconnected;

        public Mutex accessMessages { get; internal set; }
        public AutoResetEvent signalementMessage { get; internal set; }
        public AutoResetEvent signalementSortie { get; internal set; }

        public bool Connected
        {
            get
            {
                if (client != null)
                {
                    return client.Connected;
                }
                return false;
            }
        }

        public MessageManagement(string address, int port)
        {
            this.address = address;
            this.port = port;
            this.messages = new Stack<Message>();
            this.accessMessages = new Mutex();
            this.signalementMessage = new AutoResetEvent(false);
            this.signalementSortie = new AutoResetEvent(false);
        }

        public void Start()
        {
            this.client = new ClientReseau(address, port);
            if (client.Connect())
            {
                client.AccessMessages = this.accessMessages;
                client.SignalementMessage = this.signalementMessage;
                client.SignalementSortie = this.signalementSortie;
                client.Messages = this.messages;
                // On prepare l'écoute du réseau
                this.miseEnPlace();
                // Démarrage du client et ouverture du Flux sous jacent
                client.Start();
                //
            }
            else
            {
                this.client = null;
            }
        }

        private void miseEnPlace()
        {

            // et le dispatch dans un autre
            ThreadStart PointEntree = new ThreadStart(this.Ecouter);
            thEcouter = new Thread(PointEntree);
            thEcouter.Start();
        }

        private void Ecouter()
        {
            WaitHandle[] attente = new WaitHandle[2];
            //
            attente[0] = this.signalementMessage;
            attente[1] = this.signalementSortie;
            while (true)
            {
                // On attend, soit le Signalement de Message, soit le Demande de Sortie
                int who = AutoResetEvent.WaitAny(attente, Timeout.Infinite);
                if (who == 1)
                {
                    // Sortie Demandée ! Ciao
                    if (this.OnClientDisconnected != null)
                        this.OnClientDisconnected(this);
                    break;
                }
                // Il y a un message en attente
                // On accède à la pile des messages
                this.accessMessages.WaitOne();
                // On lit le message en pile
                Message msg = null;
                while (this.messages.Count > 0)
                {
                    msg = this.messages.Pop();
                    //
                    if (this.OnMessageReceived != null)
                    {
                        this.OnMessageReceived(this, msg);
                    }
                }
                // On libère
                this.accessMessages.ReleaseMutex();
            }
            //
        }

        public void Send(string msg)
        {
            if (this.client != null)
            {
                Message newMessage = new Message(this.client.Id, msg);
                //
                this.client.Ecrire(newMessage.Data);
            }
        }


        public void Stop()
        {
            if (this.client != null)
            {
                this.client.Disconnect();
            }
        }

        public bool IsRunning
        {
            get { return (client != null) && (client.IsRunning); }
        }

    }
}
