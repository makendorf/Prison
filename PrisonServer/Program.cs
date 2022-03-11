using ENet.Managed;
using Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace PS
{
    class Program
    {
        #region CloseServer
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            server.Stop();
            Thread.Sleep(3000);
            return true;
        }
        #endregion

        private static PrisonServer server;
        readonly static string ShopConnectionList = $@"{Directory.GetCurrentDirectory()}\data\cfg\ShopList.ass";

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            Thread serverThread = new Thread(new ThreadStart(StartServer));
            serverThread.Start();
            serverThread.Join();
            
        }
        static void StartServer()
        {
            server = new PrisonServer(IPAddress.Any, 2307);
            server.OnStart += ServerStarted;
            server.OnReceive += ReceivedData;
            server.OnClientConnected += ClientConnected;
            server.OnClientDisconnected += ClientDisconnected;
            server.Start();
        }

        static void ServerStarted()
        {
            CreateReservedSlots(ref server.GetClients());
        }
        private static void CreateReservedSlots(ref List<Info> clients)
        {
            if (File.Exists(ShopConnectionList))
            {
                var shopList = JsonConvert.DeserializeObject<List<ClientID>>(File.ReadAllText(ShopConnectionList));

                foreach (var shop in shopList)
                {
                    shop.IsOnline = false;
                    clients.Add(new Info
                    {
                        Client = new TcpClient(),
                        ID = shop
                    });
                }
                foreach (var item in clients)
                {
                    Log.Success($"{item.ID.GUID}, {item.ID.DisplayName}, {item.ID.Name}, {(item.ID.IsOnline ? "Online" : "Offline")}");
                    if (item.ID.Services != null)
                    {
                        foreach (var item2 in item.ID.Services)
                        {
                            Log.Success($"Служба: {item2.Name}");
                        }
                    }
                    if(item.ID.ShopBoxList != null)
                    {
                        foreach (var item2 in item.ID.ShopBoxList)
                        {
                            Log.Success($"Касса: {item2.Name} {item2.IP}:{item2.Port}");
                        }
                    }
                }
            }
        }
        static void ClientConnected(ref Info client)
        {
            Log.Warning("Клиент подключен");
        }

        static void ReceivedData(NetworkPayload Packet, ref Info client)
        {
            if(Packet.Type != PacketType.Ping)
            {
                Log.Info($"Тип пакета: {Packet.Type} Отправитель: {client.ID.Type} /  {client.ID.Name} / {client.ID.GUID}");
            }
            switch (Packet.Type)
            {
                case PacketType.ClientID:
                    {
                        var ID = (ClientID)NetworkSerialization.Deserialize(Packet.Data);
                        switch (ID.Type)
                        {
                            case ClientType.Client:
                                {
                                    var listPeer = server.GetClients();
                                    for (int i = 0; i < listPeer.Count; i++)
                                    {
                                        if (ID.Type == ClientType.Client)
                                        {
                                            if (client.Client == listPeer[i].Client)
                                            {
                                                Info CurrentVal;
                                                CurrentVal = listPeer[i];
                                                CurrentVal.Client = client.Client;
                                                ID.GUID = Guid.NewGuid().ToString();
                                                ID.IsOnline = true;
                                                CurrentVal.ID = ID;
                                                listPeer[i] = CurrentVal;

                                                NetworkPayload payload = new NetworkPayload
                                                {
                                                    Type = PacketType.ClientID,
                                                    Data = NetworkSerialization.Serialize(listPeer[i].ID)
                                                };
                                                server.Send(client, NetworkSerialization.Serialize(payload));
                                                Log.Success($"{listPeer[i].ID.Type} {listPeer[i].ID.Name} GUID: {listPeer[i].ID.GUID} зарегистрирован в системе.");
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case ClientType.Service:
                                {
                                    var listPeer = server.GetClients();
                                    Info CurrentVal = new Info();
                                    int _ID = 100;
                                    for (int i = 0; i < listPeer.Count; i++)
                                    {
                                        if (ID.GUID == listPeer[i].ID.GUID)
                                        {
                                            _ID = i;
                                            CurrentVal = listPeer[i];
                                            CurrentVal.Client = client.Client;
                                            CurrentVal.ID.IsOnline = true;
                                            listPeer[i] = CurrentVal;

                                            NetworkPayload payload = new NetworkPayload
                                            {
                                                Type = PacketType.ClientID,
                                                Data = NetworkSerialization.Serialize(listPeer[i].ID)
                                            };
                                            server.Send(client, NetworkSerialization.Serialize(payload));
                                        }
                                    }
                                    for(int i = listPeer.Count - 1; i >= 0; i--)
                                    {
                                        if (client.Client == listPeer[i].Client)
                                        {
                                            CurrentVal.Client = client.Client;
                                            CurrentVal.State = listPeer[i].State;
                                            CurrentVal.Handler = listPeer[i].Handler;
                                            listPeer[_ID] = CurrentVal;
                                            Log.Success($"{listPeer[_ID].ID.DisplayName}: {(listPeer[_ID].ID.IsOnline ? "Online" : "Offline")}");
                                            listPeer.RemoveAt(i);
                                            break;
                                        }
                                    }
                                    ConfimJSON();
                                    break;
                                }
                        }
                        break;
                    }
                case PacketType.RequestUserList:
                    {
                        var UserList = JsonConvert.DeserializeObject<UserList[]>(File.ReadAllText($@"{Directory.GetCurrentDirectory()}\data\cfg\Autorization.ass"));
                        NetworkPayload payload = new NetworkPayload
                        {
                            Type = PacketType.RequestUserList,
                            Data = NetworkSerialization.Serialize(UserList)
                        };
                        server.Send(client, NetworkSerialization.Serialize(payload));
                        break;
                    }
                case PacketType.ShopConnectionList:
                    {
                        server.Send(client, CreateShopList());
                        break;
                    }
                case PacketType.ServiceListRequest:
                    {
                        ServiceInfo[] ServiceList = null;
                        var listPeer = server.GetClients();
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (client.Client == listPeer[i].Client)
                            {
                                try
                                {
                                    ServiceList = listPeer[i].ID.Services;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex.ToString());
                                }
                                break;
                            }
                        }
                        var Response = new NetworkPayload(PacketType.ServiceListRequest, NetworkSerialization.Serialize(ServiceList));
                        server.Send(client, NetworkSerialization.Serialize(Response));
                        break;
                    }
                case PacketType.ServiceStatus:
                    {
                        var servInfo = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        var listPeer = server.GetClients();
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].Client == client.Client)
                            {
                                var cl = listPeer[i];
                                for (int j = 0; j < cl.ID.Services.Length; j++)
                                {
                                    if (servInfo.Name == cl.ID.Services[j].Name)
                                    {
                                        cl.ID.Services[j] = servInfo;
                                        listPeer[i] = cl;
                                        ConfimJSON();
                                        break;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].ID.Type == ClientType.Client)
                            {
                                NetworkPayload payload = new NetworkPayload()
                                {
                                    Data = NetworkSerialization.Serialize(servInfo),
                                    Type = PacketType.ServiceStatus
                                };
                                server.Send(listPeer[i], NetworkSerialization.Serialize(payload));
                            }
                        }
                        break;
                    }
                case PacketType.ChangeServiceAutoStart:
                    {
                        var listPeer = server.GetClients();
                        var SC = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].ID.GUID == Packet.Receiver)
                            {
                                var clID = listPeer[i];
                                for (int j = 0; j < clID.ID.Services.Length; j++)
                                {
                                    if (clID.ID.Services[j].Name == SC.Name)
                                    {
                                        clID.ID.Services[j].AutoStart = SC.AutoStart;
                                        listPeer[i] = clID;
                                        ConfimJSON();
                                        Packet.Data = NetworkSerialization.Serialize(listPeer[i].ID.Services[j]);
                                        server.Send(listPeer[i], NetworkSerialization.Serialize(Packet));
                                    }
                                }
                            }
                        }
                        break;
                    }
                case PacketType.StartService:
                    {
                        var listPeer = server.GetClients();
                        var SC = (string)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].ID.GUID == Packet.Receiver)
                            {
                                Packet.Data = NetworkSerialization.Serialize(SC);
                                server.Send(listPeer[i], NetworkSerialization.Serialize(Packet));
                            }
                        }
                        break;
                    }
                case PacketType.StopService:
                    {
                        var listPeer = server.GetClients();
                        var SC = (string)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].ID.GUID == Packet.Receiver)
                            {
                                Packet.Data = NetworkSerialization.Serialize(SC);
                                server.Send(listPeer[i], NetworkSerialization.Serialize(Packet));
                            }
                        }
                        break;
                    }
                case PacketType.RestartService:
                    {
                        var listPeer = server.GetClients();
                        var SC = (string)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].ID.GUID == Packet.Receiver)
                            {
                                Packet.Data = NetworkSerialization.Serialize(SC);
                                server.Send(listPeer[i], NetworkSerialization.Serialize(Packet));
                            }
                        }
                        break;
                    }
                case PacketType.Ping:
                    {
                        break;
                    }
            }
        }
        private static byte[] CreateShopList()
        {
            var shopList = new BindingList<ClientID>();
            foreach (var item in server.GetClients())
            {
                if (item.ID.Type == ClientType.Service)
                {
                    shopList.Add(item.ID);
                }
            }
            NetworkPayload payload = new NetworkPayload
            {
                Type = PacketType.ShopConnectionList,
                Data = NetworkSerialization.Serialize(shopList)
            };
            return NetworkSerialization.Serialize(payload);
        }
        private static List<ClientID> ConfimJSON()
        {
            var listPeer = server.GetClients();
            List<ClientID> listClients = new List<ClientID>();
            for (int i = 0; i < listPeer.Count; i++)
            {
                if (listPeer[i].ID.Type == ClientType.Service)
                {
                    listClients.Add(listPeer[i].ID);
                }
            }
            File.WriteAllText(ShopConnectionList, JsonConvert.SerializeObject(listClients, Formatting.Indented));
            return listClients;
        }

        static void ClientDisconnected(ref Info client)
        {
            var listPeer = server.GetClients();
            for (int i = 0; i < listPeer.Count; i++)
            {
                if(client.Client == listPeer[i].Client)
                {
                    Console.WriteLine("Disconnected: {0}", listPeer[i].ID.DisplayName);
                    if (listPeer[i].ID.Type == ClientType.Client)
                    {
                        listPeer.RemoveAt(i);
                        break;
                    }
                    if(listPeer[i].ID.Type != ClientType.Service)
                    {
                        listPeer[i].DropConnection();
                        var CurrentVal = listPeer[i];
                        CurrentVal.ID.IsOnline = false;
                        listPeer[i] = CurrentVal;
                        break;
                    }
                }
            }
        }
    }
}
