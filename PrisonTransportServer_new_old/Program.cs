using ENet.Managed;
using Network;
using System;
using System.Collections.Generic;
using System.Net;

namespace PrisonTransportServer
{
    class Program
    {
        static readonly ENetServer Server = new ENetServer(8);
        static readonly List<Info> listPeer = new List<Info>();


        static void Main(string[] args)
        {

            //var Server = new ENetServer(8);
            //

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
        
        private static void SendShopList()
        {
            
        }

        private static void ClientDisconnected(ENetPeer Client)
        {
            Console.WriteLine("Клиент: " + GetClientInfo(Client)?.ID.ToString() + " отключился");
            for (int i = 0; i < listPeer.Count; i++)
            {
                if (Client == listPeer[i].Client)
                {
                    listPeer.RemoveAt(i);
                }
            }
            SendShopList();
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

        private static void OnReceive(int Channel, NetworkPayload Packet, ENetPeer Sender)
        {
            Info? ClientInfo = GetClientInfo(Sender);
            if (ClientInfo == null)
            {
                Console.WriteLine("Клиент прислал сообщение, но невозможно распознать самого клиента");
                return;
            }
            Console.WriteLine("Сообщение от клиента: " + ClientInfo?.ID.ToString());
            Console.WriteLine("Тип данных: {0} Размер данных: {1}, Получатель: {2}", Packet.Type, Packet.Data.Length, Packet.Receiver == Global.ServerID ? "Сервер" : Packet.Receiver);

            if (Packet.Receiver != Global.ServerID)
            {
                //Отправка данных другим (группам) клиентам.
                switch (Packet.Receiver)
                {
                    case "AllClients":
                        {
                            Log.Info($"Отправка пакета всем клиентам");
                            foreach (var Peer in listPeer)
                            {
                                if (Peer.ID.Type == ClientType.Client)
                                {
                                    Server.Send(Peer.Client, NetworkSerialization.Serialize(Packet));
                                    Log.Info($"Отправлено: {Peer.ID.Name}");
                                }
                            }
                            return;
                        }
                    case "AllServices":
                        {
                            Log.Info($"Отправка пакета всем службам");
                            foreach (var Peer in listPeer)
                            {
                                if (Peer.ID.Type == ClientType.Service)
                                {
                                    Server.Send(Peer.Client, NetworkSerialization.Serialize(Packet));

                                    Log.Info($"Отправлено: {Peer.ID.Name}");
                                }
                            }
                            return;
                        }
                    default:
                        {
                            //Если получателя нет в базе сервера, выходим.
                            Info? ReceiverInfo = GetClientInfo(Packet.Receiver);
                            if (ReceiverInfo != null)
                            {
                                Server.Send((ENetPeer)ReceiverInfo?.Client, NetworkSerialization.Serialize(Packet));
                            }
                            return;
                        }
                }

            }

            switch (Packet.Type)
            {
                case PacketType.ClientID:
                    {
                        var ID = (ClientID)NetworkSerialization.Deserialize(Packet.Data);
                        Info CurrentVal;
                        for (int i = 0; i < listPeer.Count; i++)
                        {
                            if (Sender == listPeer[i].Client)
                            {
                                CurrentVal = listPeer[i];
                                CurrentVal.ID.Name = ID.Name;
                                CurrentVal.ID.Type = ID.Type;
                                Log.Info("Клиент присвоил себе имя: {0} ({1})", ID.Name, ID.Type);
                                if (ID.ChangeIdenififer)
                                {
                                    string OldID = ClientInfo?.ID.ToString();
                                    CurrentVal.ID.ID = ID.ToString();
                                    Log.Info("Клиент поменял ID {0} ----> {1}", OldID, listPeer[i].ID.ToString());
                                }
                                listPeer[i] = CurrentVal;
                            }
                        }
                        SendShopList();
                        break;
                    }
                case PacketType.ShopConnectionList:
                    {
                        SendShopList();
                        break;
                    }
                
            }
        }
        private static void ClientConnected(ENetPeer Client)
        {
            //Генерирует уникальные идентификаторы наподобие:
            // 0f8fad5b-d9cb-469f-a165-70867728950e
            // 7c9e6679-7425-40de-944b-e07fc1f90ae7
            var clientInfo = new Info(Client);
            listPeer.Add(clientInfo);
            switch (clientInfo.ID.Type)
            {
                case ClientType.Client:
                    {
                        NetworkPayload payload = new NetworkPayload
                        {
                            Type = PacketType.ClientID,
                            Data = NetworkSerialization.Serialize(clientInfo.ID)
                        };
                        Server.Send(Client, NetworkSerialization.Serialize(payload));
                        break;
                    }
                case ClientType.Service:
                    {

                        break;
                    }
            }
            Log.Info($"Клиент (GUID <{clientInfo.ID}>)  {Client.GetRemoteEndPoint()} подключился");
        }
    }
}
