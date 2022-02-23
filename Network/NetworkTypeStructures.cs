using Hangfire.Annotations;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Network
{
    [Serializable]
    public class ServiceConfig
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public ServiceConfig()
        {
            IP = "127.0.0.1";
            Port = 2307;
        }
    }
    [Serializable]
    public class UserList : INotifyPropertyChanged
    {
        private string password { get; set; }
        private string name { get; set; }
        private string displayName { get; set; }
        public string Password
        {
            get => password;
            set
            {
                if (value == password) return;
                password = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => name;
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged();
            }
        }
        public string DisplayName
        {
            get => displayName;
            set
            {
                if (value == displayName) return;
                displayName = value;
                OnPropertyChanged();
            }
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

    public enum PacketType
    {
        None = 0,
        Info,
        ClientID,
        ClientDisconected,
        ServiceStatus,
        ServiceStop,
        ServiceStart,
        ServiceListRequest,
        ShopConnectionList,
        RequestUserList,
        RequestAutorization,
        ChangeClientIDServiceList,
        ChangeServiceAutoStart,
        StartService,
        StopService,
        RestartService
    }
    [Serializable]
    public struct NetworkPayload
    {
        public string Sender;
        public string Receiver;
        public PacketType Type;
        public byte[] Data;

        public NetworkPayload(PacketType type, byte[] data, string sender, string receiver)
        {
            Type = type;
            Data = data;
            Sender = sender;
            Receiver = receiver;
        }
        public NetworkPayload(PacketType type, byte[] data)
        {
            Type = type;
            Data = data;
            Sender = "";
            Receiver = "";
        }
        public NetworkPayload(PacketType type)
        {
            Type = type;
            Data = new byte[0];
            Sender = "";
            Receiver = "";
        }
        public void Swap()
        {
            string _ = Sender;
            Sender = Receiver;
            Receiver = _;
        }
    }
    public class NetworkSerialization
    {
        public static byte[] Serialize(object Object)
        {
            byte[] bytes;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, Object);
                bytes = stream.ToArray();
            }
            return bytes;
        }

        public static object Deserialize(byte[] Data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(Data))
            {
                return formatter.Deserialize(stream);
            }
        }
    }
}
