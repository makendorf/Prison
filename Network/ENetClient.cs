using System;
using System.Timers;
using System.Net;
using ENet.Managed;
using System.IO;
//using System.Windows.Forms;

namespace Network
{
    public class ENetClient
    {
        public ClientID ID = new ClientID();
        
        const int MaximumPeers = 64;
        readonly byte MaximumChannels = 8;

        //const int ConnectionTries = 10;
        const int ConnectionTimeoutS = 1;

        System.Timers.Timer UpdateTimer;

        public delegate void ReceiveDelegate(int Channel, NetworkPayload Packet, ENetPeer Sender);
        public delegate void ConnectionEvent();

        public event ReceiveDelegate OnReceive;
        public event ConnectionEvent OnConnect;
        public event ConnectionEvent OnDisconnect;
        public event ConnectionEvent OnConnectionRetry;
        public event ConnectionEvent OnConnectionFailure;

        ENetHost Host;
        ENetPeer Peer;

        public ENetClient(byte NumChannels = 8)
        {
            MaximumChannels = NumChannels;
        }

        public void Disconnect()
        {
            Peer.Disconnect(0);
        }
        private bool ConnectionTry(IPEndPoint connectionEndPoint, byte MaximumChannels, uint connectionData)
        {
            Host = new ENetHost(null, MaximumPeers, MaximumChannels);
            Peer = Host.Connect(connectionEndPoint, MaximumChannels, connectionData);
            ENetEvent Event;
            bool ConnectionResult = false;
            try
            {
                Event = Host.Service(TimeSpan.FromSeconds(ConnectionTimeoutS));
                if (Event.Type == ENetEventType.Connect)
                {
                    ConnectionResult = true;
                }
            }
            catch (ENetException)
            {
                ConnectionResult = false;
                Peer.Reset();
                Host.Flush();
                Host.Dispose();

            }
            return ConnectionResult;
        }
        public void Connect(string IP, int Port)
        {
            bool Connected = false;
            Log.Info("Запуск клиента...");
            try
            {
                ManagedENet.Startup();
            }
            catch(Exception exc)
            {
                Log.Error(exc.Message);
                using(StreamWriter q = new StreamWriter(@"C:\123.txt")){
                    q.WriteLine(exc.StackTrace);
                    q.WriteLine(exc.Source);
                    q.WriteLine(exc);
                }
                Log.Error(exc.StackTrace);
            }
            //Application.DoEvents();
            Log.Info("Проверяем подключение...");
            IPEndPoint connectionEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            uint connectionData = 0;
            Log.Info("Соединение с сервером...");
            //Application.DoEvents();
            Connected = ConnectionTry(connectionEndPoint, MaximumChannels, connectionData);
            //Application.DoEvents();
            while (!Connected)
            {
                    Log.Warning("Сервер недоступен, повторное подключение...");
                    //Application.DoEvents();
                    if (ConnectionTry(connectionEndPoint, MaximumChannels, connectionData))
                    {
                        Connected = true;
                        break;
                    }
                    else
                    {
                        OnConnectionRetry?.Invoke();
                        //Application.DoEvents();
                    }
            }

            if (Connected)
            {
                UpdateTimer = new System.Timers.Timer();
                UpdateTimer.Elapsed += Update;
                UpdateTimer.Interval = 30;
                UpdateTimer.Enabled = true;
                OnConnect?.Invoke();
                Log.Success("Успешное соединение с сервером");
            }
            else
            {
                OnConnectionFailure?.Invoke();
                Log.Error("Ошибка подключениея к серверу");
            }
        }

        private void SendImpl(byte[] Data, byte Channel = 0, bool Reliable = true)
        {
            Peer.Send(Channel, Data, Reliable ? ENetPacketFlags.Reliable : ENetPacketFlags.UnreliableFragment);
        }

        public void Send(NetworkPayload Payload, byte Channel = 0, bool Reliable = true)
        {
            Payload.Sender = ID.ToString();
            if(String.IsNullOrEmpty(Payload.Receiver))
            {
                Payload.Receiver = Global.ServerID;
            }
            SendImpl(NetworkSerialization.Serialize(Payload), Channel, Reliable);
        }

        public void Send(byte[] Data, PacketType Type, byte Channel = 0, bool Reliable = true, string Receiver = Global.ServerID)
        {
            NetworkPayload payload = new NetworkPayload(Type, Data)
            {
                Sender = ID.ToString(),
                Receiver = Receiver
            };
            SendImpl(NetworkSerialization.Serialize(payload), Channel, Reliable);
        }

        private void Update(object sender, ElapsedEventArgs e)
        {
            var Event = Host.Service(TimeSpan.FromMilliseconds(10));
            switch (Event.Type)
            {
                case ENetEventType.None:
                    break;

                case ENetEventType.Connect:
                    OnConnect?.Invoke();
                    break;

                case ENetEventType.Disconnect:
                    UpdateTimer.Stop();
                    OnDisconnect?.Invoke();
                    break;
                case ENetEventType.Receive:
                    OnReceive?.Invoke(Event.ChannelId, (NetworkPayload)NetworkSerialization.Deserialize(Event.Packet.Data.ToArray()), Event.Peer);
                    Event.Packet.Destroy();
                    break;
            }
        }

        
    }
}
