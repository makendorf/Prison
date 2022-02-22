using ENet.Managed;
using Hangfire.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Network
{
    public enum ClientType
    {
        Unknown = 0,
        Service = 1,
        Client  = 2
    }

    [Serializable]
    public class ClientID : INotifyPropertyChanged
    {
        //  GUID, присваиваемый сервером при соединении
        private string guid;
        //  Сервисное имя клиента
        private string name = "Undefined";
        //  Отображаемое имя клиента
        private string displayName = "Undefined";
        public bool IsOnline { get; set; } = false;
        // Тип клиента служба/клиент
        public ClientType Type;
        private ServiceInfo[] services = null;
        public ServiceInfo[] Services
        {
            get => services;
            set
            {
                if (value == services) return;
                services = value;
                OnPropertyChanged("Services");
            }
        }
        public string GUID
        {
            get => guid;
            set
            {
                if (value == guid) return;
                guid = value;
                OnPropertyChanged("GUID");
            }
        }
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
        public ClientID()
        {
            Type = ClientType.Unknown;
        }
        public ClientID(string id)
        {
            GUID = id;
        }

        public override string ToString() => GUID;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    [Serializable]
    public struct Info
    {
        public ENetPeer Client;
        public ClientID ID;
        public Info(ENetPeer client, ClientID clientID = null) : this()
        {
            if (clientID == null) 
            {
                ID = new ClientID();
            }
            else
            {
                ID = clientID;
            }
            Client = client;
        }

    }
   
}
