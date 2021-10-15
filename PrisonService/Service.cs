using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceProcess;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Data;
using Network;
using System.IO;
using System.Management;

namespace PrisonService
{

    [Serializable]
    public partial class Service
    {
        public ServiceInfo Info;

        public string Name { get { return Info.Name; } }

        public delegate void StatusChangedDelegate(ServiceInfo Info);
        public event StatusChangedDelegate OnStatusChanged;

        private readonly ServiceController SC;
        private readonly Timer StatusCheckTimer;


        public Service(ServiceInfo info)
        {
            Info = info;
            SC = new ServiceController(info.Name);
            StatusCheckTimer = new Timer();
            StatusCheckTimer.Elapsed += CheckStatus;
            StatusCheckTimer.Interval = 100;//info.CheckInterval;
            StatusCheckTimer.Enabled = true;
            OnStatusChanged += Service_OnStatusChanged;
        }

        private void Service_OnStatusChanged(ServiceInfo Info)
        {
            if (Info.Status == Network.ServiceControllerStatus.Stopped)
            {
                Start();
            }
        }

        public System.ServiceProcess.ServiceControllerStatus GetControllerStatus()
        {
            SC.Refresh();
            return SC.Status;
        }

        public void Start()
        {
            if (Info.Status != Network.ServiceControllerStatus.Running && Info.AutoStart == true)
            {
                Log.Info($"Э! {SC.ServiceName}, ты че, совсем охуел? А ну работать!");
                try
                {
                    SC.Start();
                }
                catch (Exception exc)
                {
                    Log.Error($"Петушара {SC.ServiceName} конкретно попал!\nВыговор: {exc.Message}");
                }
            }
        }

        public void Stop()
        {
            Log.Info("{0}, иди покури.", SC.ServiceName);
            try
            {
                SC.Stop();
            }
            catch (Exception exc)
            {
                Log.Error($"Петушара {SC.ServiceName} конкретно попал!\nВыговор: {exc.Message}");
            }
        }

        public void Pause()
        {
            SC.Pause();
        }

        public void Continue()
        {
            SC.Continue();
        }

        public void Refresh()
        {
            SC.Refresh();
        }

        public void CheckStatus(object sender, ElapsedEventArgs e)
        {
            Network.ServiceControllerStatus Status = (Network.ServiceControllerStatus)GetControllerStatus();
            if (Info.Status != Status)
            {
                Info.Status = Status;
                OnStatusChanged?.Invoke(Info);
            }
        }


    }
    public partial class ServiceManager
    {
        public List<Service> Services;
        public ENetClient Client;

        public ServiceManager(ServiceInfo[] Infos, ENetClient client)
        {
            Log.Info("Пацаны, шухер! Главпетух в хате!");
            Services = new List<Service>();
            Client = client;
            foreach (var info in Infos)
            {
                var service = new Service(info);
                service.OnStatusChanged += StatusChanged;
                Services.Add(service);
            }
        }

        private void StatusChanged(ServiceInfo Info)
        {
            Log.Info("Петушара {0} сменил свою масть на {1}", Info.Name, Info.Status);
            NetworkPayload payload = new NetworkPayload(PacketType.ServiceStatus, NetworkSerialization.Serialize(Info), Client.ID.ToString(), "AllClients");
            Client.Send(payload);
        }

        public void StartAllServices()
        {
            Log.Info("Пытаемся разбудить петушар");
            foreach (var service in Services)
            {
                if (service.GetControllerStatus() != System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    Log.Info("Петушара {0} послан чистить толчок", service.Name);
                    service.Start();
                }
            }
        }

        public void StopAllServices()
        {
            foreach (var service in Services)
            {
                if (service.GetControllerStatus() == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    service.Stop();
                }
            }
        }


        public Service FindService(string Name)
        {
            Service result = null;
            foreach (var service in Services)
            {
                if (service.Name == Name)
                {
                    result = service;
                    break;
                }
            }
            return result;
        }

        public void StartService(string ServiceName)
        {
            Service service = FindService(ServiceName);
            if (service != null && service.GetControllerStatus() != System.ServiceProcess.ServiceControllerStatus.Running)
            {
                //service.Info.AutoStart = true;
                AutoStart(ServiceName, true);
                service.Start();
            }
        }

        public void StopService(string ServiceName)
        {
            Service service = FindService(ServiceName);
            if (service != null && service.GetControllerStatus() == System.ServiceProcess.ServiceControllerStatus.Running)
            {
                AutoStart(ServiceName, false);
                //service.Info.AutoStart = false;
                service.Stop();
            }
        }
        public void AutoStart(string ServiceName, bool check)
        {
            Log.Info("Атостарт");
            Service service = FindService(ServiceName);
            service.Info.AutoStart = check;
            NetworkPayload payload = new NetworkPayload(PacketType.ServiceAutoStart, NetworkSerialization.Serialize(service.Info), Client.ID.ToString(), "AllClients");
            Client.Send(payload);
        }
    }
}
