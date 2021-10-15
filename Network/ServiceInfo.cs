using System;
using MySql.Data;
using System.Collections.Generic;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Network
{
    [Serializable]
    public partial class ServiceInfo
    {

        public string Name;
        public long CheckInterval;
        public bool AutoStart;
        public ServiceControllerStatus Status;

        public ServiceInfo(string name, long checkInterval, bool autoStart = true)
        {
            Name = name;
            CheckInterval = checkInterval;
            Status = ServiceControllerStatus.UndefinedStatus;
            AutoStart = autoStart;
        }

        public static List<ServiceInfo> RequestServerList(string name)
        {
            List<ServiceInfo> serviceList = new List<ServiceInfo>();
            using (MySqlDataReader reader = MySQL.GetDataReader($"select `service`, `interval`, `server` from service"))
            {
                while (reader.Read())
                {
                    string resiver = reader.GetString(2);
                    if(resiver == name || resiver == "ALL")
                    {
                        serviceList.Add(new ServiceInfo(reader.GetString(0),
                        (long)reader.GetDateTime(1).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds));
                    }
                }
            }
            return serviceList;
        }
    }
}
