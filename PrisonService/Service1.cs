using ENet.Managed;
using Network;
using System.IO;
using System.ServiceProcess;
using Newtonsoft.Json;

namespace PrisonService
{
    public class ServiceConfig
    {
        public string ShopIdentifier { get; set; }
        public string Name { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }

        public ServiceConfig()
        {
            ShopIdentifier = "Undefined";
            Name = "Undefined";
            ServerAddress = "127.0.0.1";
            ServerPort = 2307;
        }
    }

    public partial class Service1 : ServiceBase
    {
        public ServiceManager Manager;
        public ENetClient client = new ENetClient(8);
        ServiceInfo[] ServiceList;
        static readonly ServiceConfig cfg = new ServiceConfig();
        public Service1()
        {
            this.CanStop = true; // службу можно остановить
            this.CanPauseAndContinue = true; // службу можно приостановить и затем продолжить
            this.AutoLog = true; // служба может вести запись в лог
            InitializeComponent();
        }

        static void WriteLog(LogLevel Level, string Message)
        {
            using (StreamWriter q = new StreamWriter("C:\\Записки с зоны.txt", true))
            {
                q.WriteLine(Message);
            }
        }

        private void ReadConfig()
        {

            string configPath = $@"D:\Prison\Shop.cfg";
            Log.Info(configPath);
            string JSON = File.ReadAllText(configPath);
            JsonConvert.PopulateObject(JSON, cfg);
            Log.Info($"{cfg.ShopIdentifier}, {cfg.ServerAddress}, {cfg.ServerPort}");
            client.ID = new ClientID
            {
                Name = cfg.ShopIdentifier,
                DisplayName = cfg.Name,
                Type = ClientType.Service
            };

        }

        protected override void OnStart(string[] args)
        {
            Log.OnLog += WriteLog;
            #region Подключение к серверу
            client.OnConnect += Connected;
            client.OnDisconnect += Disconnected;
            client.OnReceive += Receive;
            Log.Info("Считываем конфиг");
            ReadConfig();
            client.Connect(cfg.ServerAddress, cfg.ServerPort);
            Log.Info("Тюремщик вышел пиздить уебков");
            #endregion

        }

        private void RequestServiceList()
        {
            Log.Info("Мой ёбаный ID = {0}", client.ID.ToString());
            client.Send(new byte[0], PacketType.ServiceListRequest);
        }
        private void Receive(int Channel, NetworkPayload Packet, ENetPeer Sender)
        {
            Log.Info($"Тип данных: {Packet.Type} Размер данных: {Packet.Data.Length}");
            switch (Packet.Type)
            {
                case PacketType.ClientID:
                    var ClID = (ClientID)NetworkSerialization.Deserialize(Packet.Data);
                    Log.Info("Получил ёбаный ID = {0}", ClID.ToString());
                    client.ID.ID = ClID.ToString();
                    RequestServiceList();
                    break;

                case PacketType.Message:
                    Message msg = (Message)NetworkSerialization.Deserialize(Packet.Data);
                    Log.Info("Слышь, чепушило, тут из воли малява пришла");
                    System.Windows.Forms.MessageBox.Show(msg.Text, msg.Name);
                    msg.Name = client.ID.Name;
                    msg.Text = "Доставленно";
                    Packet.Swap();
                    Packet.Data = NetworkSerialization.Serialize(msg);
                    client.Send(Packet);
                    break;

                case PacketType.ServiceListRequest:
                    ServiceList = (ServiceInfo[])NetworkSerialization.Deserialize(Packet.Data);
                    Log.Info("ЕБАТЬ МОЙ ХУЙ, ДА ТУТ СЕРВИСЫ ЕСТЬ АЖ {0} ШТУКИ", ServiceList.Length);
                    for (int i = 0; i < ServiceList.Length; i++)
                    {
                        Log.Info("Имя петуха: {0}", ServiceList[i].Name);
                        Log.Info("Как часто его петушить: {0}", ServiceList[i].CheckInterval);
                    }
                    Manager = new ServiceManager(ServiceList, client);
                    Manager.StartAllServices();
                    break;
                case PacketType.ServiceListGet:
                    Packet.Swap();
                    Packet.Data = NetworkSerialization.Serialize(ServiceList);
                    client.Send(Packet);
                    break;
                case PacketType.ServiceStart:
                    var _srvStartInfo = (string)NetworkSerialization.Deserialize(Packet.Data);
                    Log.Info("Имя петуха: {0}, Команда: Запустить, Отправитель: {1}", _srvStartInfo, Packet.Sender);
                    Service srv = Manager.FindService(_srvStartInfo);
                    Manager.AutoStart(srv.Info.Name, true);
                    Manager.StartService(srv.Info.Name);
                    
                    break;
                case PacketType.ServiceStop:
                    var _srvStopInfo = (string)NetworkSerialization.Deserialize(Packet.Data);
                    Log.Info("Имя петуха: {0}, Команда: Остановить, Отправитель: {1}", _srvStopInfo, Packet.Sender);
                    Service _srv = Manager.FindService(_srvStopInfo);
                    Manager.AutoStart(_srv.Info.Name, false);
                    Manager.StopService(_srv.Info.Name);
                   
                    break;
                case PacketType.ServiceAutoStart:
                    {
                        var _srvs = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        Manager.AutoStart(_srvs.Name, _srvs.AutoStart);
                        break;
                    }
            }
        }



        private void SendID()
        {
            ClientID Identifier = client.ID;
            NetworkPayload payload = new NetworkPayload
            {
                Type = PacketType.ClientID,
                Data = NetworkSerialization.Serialize(Identifier)
            };
            client.Send(payload);
        }

        private void Disconnected()
        {
            Log.Info("Соединение с сервером разорвано");
            client.Connect(cfg.ServerAddress, cfg.ServerPort);
        }
        private void Connected()
        {
            Log.Info("Соединение с сервером установлено");
            SendID();
        }
        protected override void OnStop()
        {
            //client.Disconnect();
            //Service.Stop();
        }
    }
}
