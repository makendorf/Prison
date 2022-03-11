using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace Network
{
    public static class ClientExtensions
    {
        public static TcpState GetState(this TcpClient tcpClient)
        {
            var foo = IPGlobalProperties.GetIPGlobalProperties()
              .GetActiveTcpConnections()
              .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint)
                                 && x.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint)
              );

            return foo != null ? foo.State : TcpState.Unknown;
        }

    };

    public class PrisonServer
    {
        public delegate void DelegateStartStop();
        public delegate void DelegeateConnection(ref Info client);
        public delegate void DelegateReceive(NetworkPayload data, ref Info client);
        public event DelegeateConnection OnClientConnected;
        public event DelegeateConnection OnClientDisconnected;
        public event DelegateReceive OnReceive;
        public event DelegateStartStop OnStart;
        private readonly IPAddress _address;
        private readonly int _port;
        private bool _running;
        private List<Info> _connections;
        readonly TcpListener listener;

        public IPAddress Address { get { return _address; } }
        public int Port { get { return _port; } }

        public PrisonServer(string addr, int port)
        {
            _running = false;
            _port = port;
            _address = IPAddress.Parse(addr);
            _connections = new List<Info>();
            listener = new TcpListener(Address, Port);
        }

        public PrisonServer(IPAddress addr, int port)
        {
            _running = false;
            _port = port;
            _address = addr;
            _connections = new List<Info>();

            listener = new TcpListener(Address, Port);
        }

        public bool IsRunning()
        {
            return _running;
        }

        public void Stop()
        {
            listener.Stop();
            _running = false;
            for (int i = 0; i < _connections.Count; i++)
            {
                if(_connections[i].State == TcpState.Established)
                {
                    _connections[i].DropConnection();
                }
            }
        }

        public void Start()
        {
            if (IsRunning())
            {
                return;
            }
            Log.Warning("Запуск сервера...");
            listener.Start();
            _running = true;
            OnStart?.Invoke();
            Log.Success($"Сервер запущен.");
            while (IsRunning())
            {
                TcpClient client = listener.AcceptTcpClient();
                Info clientInfo = new Info(client)
                {
                    State = ClientExtensions.GetState(client)
                };
                Thread handler = new Thread(new ParameterizedThreadStart(HandleClient));
                clientInfo.Handler = handler;
                _connections.Add(clientInfo);
                handler.Start(clientInfo);
                OnClientConnected?.Invoke(ref clientInfo);
                
            }
        }
        public ref List<Info> GetClients()
        {
            return ref _connections;
        }
        public void Send(Info client, byte[] data)
        {
            if (client.State != TcpState.Established)
            {
                Log.Info(client.State.ToString());
                return;
            }
            try
            {
                client.GetSocket().Send(data);
            }
            catch (Exception e)
            {
                client.DropConnection();
                OnClientDisconnected?.Invoke(ref client);
                Log.Error("Can't send message: ", e.Message);
            }
        }


        private void HandleClient(object obj)
        {
            Info client = (Info)obj;
            NetworkStream stream = client.Client.GetStream();
            client.State = ClientExtensions.GetState(client.Client);
            while (client.State == TcpState.Established)
            {
                if (ClientExtensions.GetState(client.Client) != client.State)
                {
                    client.State = ClientExtensions.GetState(client.Client);
                }
                if (client.State == TcpState.Established && stream.DataAvailable)
                {
                    byte[] data = new byte[client.Client.Available];
                    try
                    {
                        int dataLen = stream.Read(data, 0, client.Client.Available);
                        byte[] payload = new byte[dataLen];
                        Array.Copy(data, payload, dataLen);
                        int id = _connections.IndexOf(client);
                        var cl = _connections[id];
                        if (id != -1)
                            OnReceive?.Invoke((NetworkPayload)NetworkSerialization.Deserialize(payload), ref cl);
                        else
                        {
                            Log.Warning("Unknown client");
                        }
                    }
                    catch (Exception)
                    {
                        client.DropConnection();
                        break;
                    }
                }
            }
            OnClientDisconnected?.Invoke(ref client);
        }
    }
}
