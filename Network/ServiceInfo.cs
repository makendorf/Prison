using System;
using MySql.Data;
using System.Collections.Generic;
using System.Threading;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using Hangfire.Annotations;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.IO;

namespace Network
{
    [Serializable]
    public partial class ServiceInfo: INotifyPropertyChanged
    {

        public string name;
        public long checkInterval;
        public bool autoStart;
        public ServiceControllerStatus status;
        public string Name
        {
            get => name;
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public long CheckInterval
        {
            get => checkInterval;
            set
            {
                if (value == checkInterval) return;
                checkInterval = value;
                OnPropertyChanged("CheckInterval");
            }
        }
        public bool AutoStart
        {
            get => autoStart;
            set
            {
                if (value == autoStart) return;
                autoStart = value;
                OnPropertyChanged("AutoStart");
            }
        }
        public ServiceControllerStatus Status
        {
            get => status;
            set
            {
                if (value == status) return;
                status = value;
                OnPropertyChanged("AutoStart");
            }
        }
        public ServiceInfo(string name, long checkInterval, bool autoStart = true)
        {
            Name = name;
            CheckInterval = checkInterval;
            Status = ServiceControllerStatus.UndefinedStatus;
            AutoStart = autoStart;
        }
        public override string ToString()
        {
            return Name;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
