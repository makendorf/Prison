using ENet.Managed;
using Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;

namespace PrisonServer
{
    class Program
    {
        static readonly ENetServer Server = new ENetServer(8);
        readonly static string ShopConnectionList = $@"{Directory.GetCurrentDirectory()}\data\cfg\ShopList.ass";
        static List<Info> listPeer = new List<Info>();
        static List<Info> listPeerTrash = new List<Info>();

        static void Main(string[] args)
        {
            CreateShopList();
            Server.OnClientConnected += ClientConnected;
            Server.OnClientDisconnected += ClientDisconnected;
            Server.OnReceive += OnReceive;

            Server.Start(IPAddress.Any, 2307);

            while (true)
            {
                string CmdLine = Console.ReadLine();
                string[] Cmd = CmdLine.Split(' ');
                if (Cmd.Length < 1) continue;

                switch (Cmd[0].ToLower())
                {
                    case "exit":
                        {
                            Environment.Exit(0);
                            break;
                        }
                    case "service": //service имя_сервиса команда сервер
                        {
                            if (Cmd.Length < 4)
                            {
                                Log.Warning("Недостаточно аргументов для выполнения команды");
                                break;
                            }
                            string service = Cmd[1];
                            string command = Cmd[2];
                            string serverName = Cmd[3];


                            break;
                        }
                    case "list":
                        {
                            if (Cmd.Length < 2)
                            {
                                foreach (var item in listPeer)
                                {
                                    Log.Success($"Имя: {item.ID.Name}, GUID: {item.ID}, Тип: {item.ID.Type}");
                                }
                                break;
                            }

                            if (Cmd[1].ToLower() == "clients")
                            {
                                foreach (var item in listPeer)
                                {
                                    if (item.ID.Type == ClientType.Client)
                                    {
                                        Log.Success($"Имя: {item.ID.Name}, GUID: {item.ID}, Тип: {item.ID.Type}");
                                    }
                                }
                                break;
                            }

                            if (Cmd[1].ToLower() == "services")
                            {
                                foreach (var item in listPeer)
                                {
                                    if (item.ID.Type == ClientType.Service)
                                    {
                                        Log.Success($"Имя: {item.ID.Name}, GUID: {item.ID}, Тип: {item.ID.Type}");
                                    }
                                }
                                break;
                            }
                            break;
                        }
                    default:
                        Log.Warning("Неизвестная команда '{0}'", Cmd[0]);
                        break;

                }

            }
        }
        
        private static byte[] SendShopList()
        {
            var shopList = new BindingList<ClientID>();
            foreach(var item in listPeer)
            {
                if(item.ID.Type == ClientType.Service)
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

        private static void CreateShopList()
        {
            var shopList = JsonConvert.DeserializeObject<List<ClientID>>(File.ReadAllText(ShopConnectionList));

            foreach (var shop in shopList)
            {
                shop.IsOnline = false;
                listPeer.Add(new Info
                {
                    Client = new ENetPeer(),
                    ID = shop
                });
            }
            foreach (var item in listPeer)
            {
                Log.Success($"{item.ID.GUID}, {item.ID.DisplayName}, {item.ID.Name}, {(item.ID.IsOnline ? "Online" : "Offline")}");
                if (item.ID.Services != null)
                    foreach (var item2 in item.ID.Services)
                    {
                        Log.Success($"Служба:{item2.Name}");
                    }
            }
        }

        private static Info? GetClientInfo(string ClientID)
        {
            for (int i = 0; i < listPeer.Count; i++)
            {
                if (ClientID == listPeer[i].ID.ToString())
                {
                    return listPeer[i];
                }
            }
            return null;
        }

        private static Info? GetClientInfo(ENetPeer Peer)
        {
            for (int i = 0; i < listPeer.Count; i++)
            {
                if (Peer == listPeer[i].Client)
                {
                    return listPeer[i];
                }
            }
            return null;
        }
        private static List<ClientID> ConfimJSON()
        {
            List<ClientID> listClients = new List<ClientID>();
            for(int i = 0;i < listPeer.Count; i++)
            {
                if(listPeer[i].ID.Type == ClientType.Service)
                {
                    listClients.Add(listPeer[i].ID);
                }
            }
            File.WriteAllText(ShopConnectionList, JsonConvert.SerializeObject(listClients, Formatting.Indented));
            return listClients;
        }
        private static void OnReceive(int Channel, NetworkPayload Packet, ENetPeer Sender)
        {
            Info? ClientInfo = GetClientInfo(Sender);
           
            Console.WriteLine("Сообщение от клиента: " + ClientInfo?.ID.ToString());
            Console.WriteLine("Тип данных: {0} Размер данных: {1}, Получатель: {2}", Packet.Type, Packet.Data.Length, Packet.Receiver == Global.ServerID ? "Сервер" : Packet.Receiver);

            switch (Packet.Type)
            {
                case PacketType.ClientID:
                    {
                        var ID = (ClientID)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeerTrash.Count; i++)
                        {
                            if(listPeerTrash[i].Client == Sender)
                            {
                                listPeerTrash.RemoveAt(i);
                                break;
                            }
                        }
                        switch (ID.Type)
                        {
                            case ClientType.Client:
                                {
                                    ClientID newID = new ClientID()
                                    {
                                        GUID = Guid.NewGuid().ToString(),
                                        DisplayName = ID.DisplayName,
                                        Name = ID.Name,
                                        Type = ID.Type,
                                        IsOnline = true
                                    };
                                    listPeer.Add(new Info(Sender, newID));
                                    Log.Info("Клиенту присвоен GUID:{1} {0} ({2})", newID.Name, newID.Type, newID.GUID);
                                    NetworkPayload payload = new NetworkPayload
                                    {
                                        Type = PacketType.ClientID,
                                        Data = NetworkSerialization.Serialize(newID)
                                    };
                                    Server.Send(Sender, NetworkSerialization.Serialize(payload));
                                    
                                    break;
                                }
                            case ClientType.Service:
                                {
                                    for (int i = 0; i < listPeer.Count; i++)
                                    {
                                        if (ID.GUID == listPeer[i].ID.GUID)
                                        {
                                            Info CurrentVal;
                                            CurrentVal = listPeer[i];
                                            CurrentVal.Client = Sender;
                                            CurrentVal.ID.IsOnline = true;
                                            listPeer[i] = CurrentVal;

                                            Log.Success($"{listPeer[i].ID.DisplayName}: {(listPeer[i].ID.IsOnline ? "Online" : "Offline")}");
                                            NetworkPayload payload = new NetworkPayload
                                            {
                                                Type = PacketType.ClientID,
                                                Data = NetworkSerialization.Serialize(listPeer[i].ID)
                                            };
                                            Server.Send(Sender, NetworkSerialization.Serialize(payload));
                                            foreach(var cli in listPeer)
                                            {
                                                if(cli.ID.Type == ClientType.Client)
                                                {
                                                    Server.Send(Sender, SendShopList());
                                                }
                                            }
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
                        Server.Send(Sender, NetworkSerialization.Serialize(payload));
                        break;
                    }
                case PacketType.ShopConnectionList:
                    {
                        Server.Send(Sender, SendShopList());
                        break;
                    }
                case PacketType.ServiceListRequest:
                    {
                        ServiceInfo[] ServiceList = null;
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (Sender == listPeer[i].Client)
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
                        Server.Send(Sender, NetworkSerialization.Serialize(Response));
                        break;
                    }
                case PacketType.ServiceStatus:
                    {
                        var servInfo = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].Client == Sender)
                            {
                               
                                for (int j = 0; j < listPeer[i].ID.Services.Length; j++)
                                {
                                    if (servInfo.Name == listPeer[i].ID.Services[j].Name)
                                    {
                                        listPeer[i].ID.Services[j] = servInfo;
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
                                Server.Send(listPeer[i].Client, NetworkSerialization.Serialize(payload));
                            }
                        }
                        break;
                    }
                case PacketType.ChangeServiceAutoStart:
                    {
                        var SC = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        for(int i = 0; i < listPeer.Count; i++)
                        {
                            if(listPeer[i].ID.GUID == Packet.Receiver)
                            {
                                for(int j = 0; j < listPeer[i].ID.Services.Length; j++)
                                {
                                    if(listPeer[i].ID.Services[j].Name == SC.Name)
                                    {
                                        listPeer[i].ID.Services[j].AutoStart = SC.AutoStart;
                                        ConfimJSON();
                                        Packet.Data = NetworkSerialization.Serialize(listPeer[i].ID.Services[j]);
                                        Server.Send(listPeer[i].Client, NetworkSerialization.Serialize(Packet));
                                    }
                                }
                            }
                        }
                        break;
                    }
                case PacketType.StartService:
                    {
                        var SC = (string)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].ID.GUID == Packet.Receiver)
                            {
                                Packet.Data = NetworkSerialization.Serialize(SC);
                                Server.Send(listPeer[i].Client, NetworkSerialization.Serialize(Packet));
                            }
                        }
                        break;
                    }
                case PacketType.StopService:
                    {
                        var SC = (string)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].ID.GUID == Packet.Receiver)
                            {
                                Packet.Data = NetworkSerialization.Serialize(SC);
                                Server.Send(listPeer[i].Client, NetworkSerialization.Serialize(Packet));
                            }
                        }
                        break;
                    }
                case PacketType.RestartService:
                    {
                        var SC = (string)NetworkSerialization.Deserialize(Packet.Data);
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (listPeer[i].ID.GUID == Packet.Receiver)
                            {
                                Packet.Data = NetworkSerialization.Serialize(SC);
                                Server.Send(listPeer[i].Client, NetworkSerialization.Serialize(Packet));
                            }
                        }
                        break;
                    }
            }
        }

        

        private static void ClientConnected(ENetPeer Client)
        {
            var clientInfo = new Info(Client);
            listPeerTrash.Add(clientInfo);
            
        }
        private static void ClientDisconnected(ENetPeer Client)
        {
            Console.WriteLine("Клиент: " + GetClientInfo(Client)?.ID.ToString() + " отключился");

            switch (GetClientInfo(Client)?.ID.Type)
            {
                case ClientType.Client:
                    {

                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if(Client == listPeer[i].Client)
                            {
                                listPeer.RemoveAt(i);
                                break;
                            }
                        }
                        break;
                    }
                case ClientType.Service:
                    {
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (Client == listPeer[i].Client)
                            {
                                Info CurrentVal;
                                CurrentVal = listPeer[i];
                                CurrentVal.Client = new ENetPeer();
                                CurrentVal.ID = GetClientInfo(Client)?.ID;
                                CurrentVal.ID.IsOnline = false;
                                listPeer[i] = CurrentVal;
                                ConfimJSON(); 
                                break;
                            }
                        }
                        break;
                    }
            }
        }
    }
}
