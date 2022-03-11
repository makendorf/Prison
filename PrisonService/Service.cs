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
using Newtonsoft.Json;
using System.Diagnostics;

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
        private Timer KillServiceTimer;



        public Service(ServiceInfo info)
        {
            Info = info;
            SC = new ServiceController(Info.Name);
            StatusCheckTimer = new Timer();
            StatusCheckTimer.Elapsed += CheckStatus;
            StatusCheckTimer.Interval = info.CheckInterval;//info.CheckInterval;
            StatusCheckTimer.Enabled = true;

            KillServiceTimer = new Timer();
            KillServiceTimer.Elapsed += KillService;
            KillServiceTimer.Interval = 60000;

            OnStatusChanged += Service_OnStatusChanged;
        }

        private void KillService(object sender, ElapsedEventArgs e)
        {
            var exeName = FindRunFile();
            try
            {
                foreach (Process proc in Process.GetProcessesByName(exeName))
                {
                    proc.Kill();
                    Log.Error($"Пришлось порешить петушару {Name}");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception exc)
            {
                Log.Error(exc.Message);
            }
        }

        private void Service_OnStatusChanged(ServiceInfo Info)
        {
            if (Info.AutoStart)
            {
                if (Info.Status == Network.ServiceControllerStatus.Stopped)
                {
                    Start();
                }
            }
            if(!KillServiceTimer.Enabled && (Info.Status != Network.ServiceControllerStatus.Stopped || Info.Status != Network.ServiceControllerStatus.Running))
            {
                Log.Error($"Петушара {Name} на грани смерти");
                KillServiceTimer.Start();
            }
            if (KillServiceTimer.Enabled && (Info.Status == Network.ServiceControllerStatus.Stopped || Info.Status == Network.ServiceControllerStatus.Running))
            {
                KillServiceTimer.Stop();
            }
        }
        public void CheckOff()
        {
            StatusCheckTimer.Stop();
        }
        public System.ServiceProcess.ServiceControllerStatus GetControllerStatus()
        {
            SC.Refresh();
            return SC.Status;
        }

        public void Start()
        {
            if (Info.Status != Network.ServiceControllerStatus.Running)
            {
                try
                {
                    SC.Start();
                    Log.Info($"Э! {SC.ServiceName}:{Info.AutoStart}, ты че, совсем охуел? А ну работать!");
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
        private string FindRunFile()
        {
            string ComputerName = "localhost";
            ManagementScope Scope;

            if (!ComputerName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                ConnectionOptions Conn = new ConnectionOptions();
                Conn.Username = "";
                Conn.Password = "";
                Conn.Authority = "ntlmdomain:DOMAIN";
                Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", ComputerName), Conn);
            }
            else
                Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", ComputerName), null);

            Scope.Connect();
            ObjectQuery Query = new ObjectQuery("SELECT * FROM Win32_Service");
            ManagementObjectSearcher Searcher = new ManagementObjectSearcher(Scope, Query);

            foreach (ManagementObject WmiObject in Searcher.Get())
            {
                if ((string)WmiObject["Name"] == Name)
                {
                    var fullPath = ((string)WmiObject["PathName"]).Split(@"\".ToCharArray());
                    var nameFile = fullPath[fullPath.Length - 1].Split(" -".ToCharArray())[0].Replace(".exe", "");
                    return nameFile;
                }
            }
            return "";
        }


    }
    public partial class ServiceManager
    {
        public List<Service> Services = new List<Service>();
        public ServiceManager()
        {
            Log.Info("Пацаны, шухер! Главпетух в хате!");
            foreach (var info in Program.service1.client.ID.Services)
            {
                Log.Info($"{info.Name}:{info.AutoStart}");
                var service = new Service(info);
                service.OnStatusChanged += StatusChanged;
                Services.Add(service);
            }
        }


        public void Dispose()
        {
            for(int i = 0; i < Services.Count; i++)
            {
                try
                {
                    Services[i].OnStatusChanged -= StatusChanged;
                    Services[i].CheckOff();
                    Services[i] = null;
                    Services.RemoveAt(i);
                }
                catch (Exception)
                {

                }
            }
            Services.Clear();
        }
        private void StatusChanged(ServiceInfo Info)
        {
            Log.Info("Петушара {0}({2}) сменил свою масть на {1}", Info.Name, Info.Status, Info.AutoStart);
           
            for(int i = 0; i < Program.service1.client.ID.Services.Length; i++)
            {
                if(Program.service1.client.ID.Services[i].Name == Info.Name)
                {
                    Program.service1.client.ID.Services[i] = Info;
                }
            }
            ApplyChange();
            NetworkPayload payload = new NetworkPayload(PacketType.ServiceStatus, NetworkSerialization.Serialize(Info));
            Program.service1.client.Send(payload);
        }

        public void StartAllServices()
        {
            Log.Info("Пытаемся разбудить петушар");
            foreach (var service in Services)
            {
                if (service.GetControllerStatus() != System.ServiceProcess.ServiceControllerStatus.Running && service.Info.AutoStart)
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
                Log.Info("Петушара {0} послан чистить толчок", service.Name);
                service.Start();
            }
        }

        public void StopService(string ServiceName)
        {
            Service service = FindService(ServiceName);
            if (service != null && service.GetControllerStatus() != System.ServiceProcess.ServiceControllerStatus.Stopped)
            {
                service.Stop();
            }
        }
        internal void RestartService(string data)
        {
            Service service = FindService(data);
            service.Stop();
            int i = 0;
            while(service.GetControllerStatus() != System.ServiceProcess.ServiceControllerStatus.Stopped)
            {
                System.Threading.Thread.Sleep(1000);
                service.Start();
            }
            
        }
        
        private void ApplyChange()
        {
            foreach(var service in Services)
            {
                for(int i = 0; i < Program.service1.client.ID.Services.Length; i++)
                {
                    if (service.Info.Name == Program.service1.client.ID.Services[i].Name)
                    {
                        Program.service1.client.ID.Services[i] = service.Info;
                    }
                }
                
            }
            Program.service1.ConfimJSON();
        }
        public void AutoStart(string ServiceName, bool autoStart, long checkInterval)
        {
            Service service = FindService(ServiceName);
            service.Info.AutoStart = autoStart;
            service.Info.CheckInterval = checkInterval;
            if (service.Info.AutoStart)
            {
                service.Start();
            }
            ApplyChange();
        }
    }
}
