using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Network
{

    public enum PacketType
    {
        None = 0,
        Info,
        ClientID,
        ServiceStatus,
        ServiceStop,
        ServiceStart,
        ServiceListRequest,
        ServiceListGet,
        ServiceAutoStart,
        ConnectionList,
        ShopConnectionList,
        Message
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
