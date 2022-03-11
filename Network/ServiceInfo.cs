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

        private string name;
        private string displayName;
        private long checkInterval;
        private bool autoStart;
        private ServiceControllerStatus status;
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
        public string DisplayName
        {
            get => displayName;
            set
            {
                if (value == displayName) return;
                displayName = value;
                OnPropertyChanged("DisplayName");
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
