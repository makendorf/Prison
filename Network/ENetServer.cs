using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Timers;
using ENet.Managed;

namespace Network
{
    public class ENetServer
    {
        int MaximumPeers = 64;
        byte MaximumChannels = 8;
        ENetHost Host;
        Timer UpdateTimer;

        public delegate void ReceiveDelegate(int Channel, NetworkPayload Packet, ENetPeer Sender);
        public delegate void ConnectionEvent(ENetPeer Client);

        public event ReceiveDelegate OnReceive;
        public event ConnectionEvent OnClientConnected;
        public event ConnectionEvent OnClientDisconnected;


        public ENetServer(byte ChannelCount = 0, int ConnectionLimit = 64)
        {
            MaximumPeers = ConnectionLimit;
            MaximumChannels = ChannelCount;
        }

        public void Stop()
        {
            UpdateTimer.Enabled = false;
            Host.Dispose();   
        }

        public void Start(IPAddress Address, int Port)
        {
            Console.WriteLine("Запуск сервера...");
            ManagedENet.Startup();

            var listenEndPoint = new IPEndPoint(Address, Port);

            Host = new ENetHost(listenEndPoint, MaximumPeers, MaximumChannels);

            Console.WriteLine($"Сервер запущен: {listenEndPoint}");
            UpdateTimer = new Timer();
            UpdateTimer.Elapsed += Update;
            UpdateTimer.Interval = 30;
            UpdateTimer.Enabled = true;
        }

        public void Broadcast(byte[] Data, byte Channel = 0, bool Reliable = true)
        {
            Host.Broadcast(Channel, Data, Reliable ? ENetPacketFlags.Reliable : ENetPacketFlags.UnreliableFragment);
        }

        public void Send(ENetPeer Peer, byte[] Data, byte Channel = 0, bool Reliable = true)
        {
            Peer.Send(Channel, Data, Reliable ? ENetPacketFlags.Reliable : ENetPacketFlags.UnreliableFragment);
        }

        private void Update(object sender, ElapsedEventArgs e)
        {
            var Event = Host.Service(TimeSpan.FromMilliseconds(10));
            switch (Event.Type)
            {
                case ENetEventType.None:
                    break;

                case ENetEventType.Connect:
                    OnClientConnected?.Invoke(Event.Peer);
                    break;

                case ENetEventType.Disconnect:
                    OnClientDisconnected?.Invoke(Event.Peer);
                    break;
                case ENetEventType.Receive:
                    OnReceive?.Invoke(Event.ChannelId, (NetworkPayload)NetworkSerialization.Deserialize(Event.Packet.Data.ToArray()), Event.Peer);
                    Event.Packet.Destroy();
                    break;
            }
        }
    }

}
