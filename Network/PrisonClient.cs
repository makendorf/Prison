using System;
using System.Timers;
using System.Net;
using ENet.Managed;
using System.IO;
using System.Net.Sockets;
using System.Threading;
//using System.Windows.Forms;

namespace Network
{
    public class PrisonClient
    {
        public delegate void DelegateConnectDisconnect();
        public delegate void DelegateReceive(NetworkPayload data);
        public event DelegateConnectDisconnect OnConnected;
        public event DelegateConnectDisconnect OnConnectionFailure;
        public event DelegateConnectDisconnect OnDisconnect;
        public event DelegateReceive OnReceive;


        private readonly IPAddress _address;
        private readonly int _port;
        private bool _running;
        private int _retries = 5;
        private int _tryNum = 0;

        private readonly TcpClient client;
        private System.Timers.Timer _timerPing;
        public IPAddress Address { get { return _address; } }
        public int Port { get { return _port; } }

        public ClientID ID;
        public Thread Handler;

        public PrisonClient(string addr, int port)
        {
            _running = false;
            _port = port;
            _address = IPAddress.Parse(addr);
            client = new TcpClient();
            ID = new ClientID();
            OnConnectionFailure += Reconnect;
            _timerPing = new System.Timers.Timer();
            _timerPing.Interval = 1000;
            _timerPing.Elapsed += _timerPing_Elapsed;
       
        }
        public PrisonClient(IPAddress addr, int port)
        {
            _running = false;
            _port = port;
            _address = addr;
            client = new TcpClient();
            ID = new ClientID();
            OnConnectionFailure += Reconnect;
        }
        private void _timerPing_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_running)
            {
                NetworkPayload ping = new NetworkPayload()
                {
                    Data = new byte[0],
                    Type = PacketType.Ping
                };
                Send(NetworkSerialization.Serialize(ping));
            }
            else
            {
                _timerPing.Stop();
            }
        }

        private void Reconnect()
        {
            //Log.Info("Ошибка соединения с сервером");
            //if(_tryNum < _retries)
            //{
            //    Log.Info("Попытка переподключиться 5 секунд...");
            //    Thread.Sleep(1000);
            //    Connect();
            //    _tryNum++;
            //}
            Log.Info("Переподключение...");
            Thread.Sleep(1000);
            Connect();
        }

        public bool IsConnected()
        {
            return client.Connected;
        }

        public void Connect()
        {
            try
            {
                Log.Info("Подключение...");
                client.Connect(Address, Port);
                _running = true;
                _timerPing.Start();
                Handler = new Thread(new ParameterizedThreadStart(HandleConnection));
                Handler.Start(client);
                OnConnected?.Invoke();
            }
            catch(Exception)
            {
                OnConnectionFailure?.Invoke();
            }

        }

        public void Disconnect()
        {
            _running = false;
            Handler.Abort();
            client.Client.Disconnect(false);
        }

        public void Send(byte[] data)
        {
            if (!IsConnected())
            {
                return;
            }
            try
            {
                client.Client.Send(data);
            }
            catch (Exception e)
            {
                Disconnect();
                Log.Error("Can't send message: " + e.Message);
                OnDisconnect?.Invoke();
            }
        }
        private void SendImpl(byte[] Data)
        {
            if (!IsConnected())
            {
                return;
            }
            try
            {
                client.Client.Send(Data);
            }
            catch (Exception e)
            {
                Log.Error("Can't send message: " + e.Message);
                Disconnect();
                OnDisconnect?.Invoke();
            }
        }

        public void Send(NetworkPayload Payload)
        {
            Payload.Sender = ID.ToString();
            if (String.IsNullOrEmpty(Payload.Receiver))
            {
                Payload.Receiver = Global.ServerID;
            }
            SendImpl(NetworkSerialization.Serialize(Payload));
        }

        public void Send(byte[] Data, PacketType Type, string Receiver = Global.ServerID)
        {
            NetworkPayload payload = new NetworkPayload(Type, Data)
            {
                Sender = ID.ToString(),
                Receiver = Receiver
            };
            SendImpl(NetworkSerialization.Serialize(payload));
        }

        private void HandleConnection(object obj)
        {
            TcpClient tcp = (TcpClient)obj;
            NetworkStream stream = tcp.GetStream();
            bool connected = IsConnected();
            while (_running)
            {
                connected = IsConnected();
                if (connected && stream.DataAvailable)
                {
                    byte[] data = new byte[tcp.Client.Available];
                    try
                    {
                        int dataLen = stream.Read(data, 0, tcp.Client.Available);
                        byte[] payload = new byte[dataLen];
                        Array.Copy(data, payload, dataLen);
                        OnReceive?.Invoke((NetworkPayload)NetworkSerialization.Deserialize(payload));
                    }
                    catch (Exception e)
                    {
                        Disconnect();
                        Log.Error("Receive error: " + e.Message);
                        break;
                    }
                }
            }
            OnDisconnect?.Invoke();

        }

    }
}
