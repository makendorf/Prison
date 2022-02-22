using ENet.Managed;
using Network;
using System.IO;
using System.ServiceProcess;
using Newtonsoft.Json;
using System;
using System.Timers;

namespace PrisonService
{


    public partial class Service1 : ServiceBase
    {
        public ServiceManager Manager = null;
        public ENetClient client = new ENetClient(8);
        static ServiceConfig cfg = new ServiceConfig();
        public Service1()
        {
            this.CanStop = true; // службу можно остановить
            this.CanPauseAndContinue = true; // службу можно приостановить и затем продолжить
            this.AutoLog = true; // служба может вести запись в лог
            InitializeComponent();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        static void WriteLog(LogLevel Level, string Message)
        {
            using (StreamWriter q = new StreamWriter("D:\\Записки с зоны.txt", true))
            {
                q.WriteLine(Message);
            }
        }

        private void ReadAuth()
        {
            try
            {
                var str = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\data\cfg\auth.ass");
                var ID = JsonConvert.DeserializeObject<ClientID[]>(str);
                Log.Success(ID[0].ToString());
                ClientID clientID = new ClientID
                {
                    GUID = ID[0].ToString(),
                    Name = ID[0].Name,
                    DisplayName = ID[0].DisplayName,
                    Type = ClientType.Service,
                    Services = ID[0].Services
                };
                client.ID = clientID;
                Log.Success(client.ID.ToString());
            }
            catch (Exception exc)
            {
                Log.Error(Directory.GetCurrentDirectory() + exc.Message + "\n" + exc.StackTrace);
            }

        }
        private void ReadServerCfg()
        {
            var config = JsonConvert.DeserializeObject<ServiceConfig[]>(File.ReadAllText($@"{Directory.GetCurrentDirectory()}\data\cfg\server.ass"));
            cfg = new ServiceConfig()
            {
                IP = config[0].IP,
                Port = config[0].Port,
            };
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Log.OnLog += WriteLog;
                #region Подключение к серверу
                client.OnConnect += Connected;
                client.OnDisconnect += Disconnected;
                client.OnReceive += Receive;
                Log.Info("Считываем GUID");
                ReadAuth();

                //Manager = new ServiceManager();
                //Manager.StartAllServices();

                Log.Info("Считываем конфиг");
                ReadServerCfg();
                client.Connect(cfg.IP, cfg.Port);

                //Timer timerReqServ = new Timer();
                //timerReqServ.Elapsed += TimerReqServ_Elapsed;
                //timerReqServ.Interval = 10000;
                //timerReqServ.Start();

                Log.Info("Тюремщик вышел пиздить уебков");
                #endregion
            }
            catch (Exception exc)
            {
                Log.Error(exc.Message + "\n" + exc.StackTrace);
            }


        }

        private void TimerReqServ_Elapsed(object sender, ElapsedEventArgs e)
        {
            RequestServiceList();
        }

        public void ConfimJSON()
        {
            if(Manager != null)
            {
                ServiceInfo[] services = new ServiceInfo[Manager.Services.Count];
                for (int i = 0; i < Manager.Services.Count; i++)
                {
                    services[i] = Manager.Services[i].Info;
                }
                client.ID.Services = services;
            }
            File.WriteAllText($@"{Directory.GetCurrentDirectory()}\data\cfg\auth.ass", "[" + JsonConvert.SerializeObject(client.ID, Formatting.Indented) + "]");
        }
       

        private void RequestServiceList()
        {
            client.Send(new byte[0], PacketType.ServiceListRequest);
        }
        private void Receive(int Channel, NetworkPayload Packet, ENetPeer Sender)
        {
            Log.Info($"Тип данных: {Packet.Type} Размер данных: {Packet.Data.Length}");
            switch (Packet.Type)
            {
                case PacketType.ClientID:
                    {
                        var data = (ClientID)NetworkSerialization.Deserialize(Packet.Data);
                        try
                        {
                            client.ID = data;
                            ConfimJSON();
                            Log.Info("Авторизация прошла успешно");
                            Manager = new ServiceManager();
                            Manager.StartAllServices();
                        }
                        catch (Exception exc)
                        {
                            Log.Error(exc.ToString());
                        }
                        break;
                    }
                case PacketType.ChangeServiceAutoStart:
                    {
                        var data = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        try
                        {
                            string log = $"Обновление данных {Manager.FindService(data.Name).Name} AutoStart ({Manager.FindService(data.Name).Info.AutoStart}) -> AutoStart (";
                            Manager.AutoStart(data.Name, data.AutoStart, data.CheckInterval);
                            log += $"{Manager.FindService(data.Name).Info.AutoStart})";
                            Log.Info(log);
                            ConfimJSON();
                        }
                        catch (Exception exc)
                        {
                            Log.Error(exc.ToString());
                        }
                        break;
                    }
                case PacketType.StartService:
                    {
                        var data = (string)NetworkSerialization.Deserialize(Packet.Data);
                        try
                        {
                            Manager.StartService(data);
                            ConfimJSON();
                        }
                        catch (Exception exc)
                        {
                            Log.Error(exc.ToString());
                        }
                        break;
                    }
                case PacketType.StopService:
                    {
                        var data = (string)NetworkSerialization.Deserialize(Packet.Data);
                        try
                        {
                            Manager.StopService(data);
                            ConfimJSON();
                        }
                        catch (Exception exc)
                        {
                            Log.Error(exc.ToString());
                        }
                        break;
                    }
                case PacketType.RestartService:
                    {
                        var data = (string)NetworkSerialization.Deserialize(Packet.Data);
                        try
                        {
                            Manager.RestartService(data);
                            ConfimJSON();
                        }
                        catch (Exception exc)
                        {
                            Log.Error(exc.ToString());
                        }
                        break;
                    }

            }
        }



        private void SendID()
        {
            NetworkPayload payload = new NetworkPayload(PacketType.ClientID, NetworkSerialization.Serialize(client.ID));
            client.Send(payload);
        }

        private void Disconnected()
        {
            Log.Info("Соединение с сервером разорвано");
            client.Connect(cfg.IP, cfg.Port);
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
