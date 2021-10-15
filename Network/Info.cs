using ENet.Managed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Network
{
    public enum ClientType
    {
        Unknown = 0,
        Service,
        Client
    }

    [Serializable]
    public class ClientID
    {
        //  GUID, присваиваемый сервером при соединении
        public string ID;
        //  Сервисное имя клиента
        public string Name = "Undefined";
        //  Отображаемое имя клиента
        public string DisplayName = "Undefined";
        // Может ли клиент изменять свой GUID ?
        public bool ChangeIdenififer = false;
        // Тип клиента служба/клиент
        public ClientType Type;
        public ClientID()
        {
            Type = ClientType.Unknown;
            ID = Guid.NewGuid().ToString();
        }
        public ClientID(string id)
        {
            ID = id;
        }

        public override string ToString()
        {
            return ID;
        }


    }



    [Serializable]
    public struct Info
    {
        public ENetPeer Client;
        public ClientID ID;
        public Info(ENetPeer client)
        {
            ID = new ClientID();
            Client = client;
        }
    }
   
}
