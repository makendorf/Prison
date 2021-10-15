using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Org.BouncyCastle.Crypto.Tls;

namespace Network
{
    public enum CommandType
    {
        Unknown = 0,
        Start,
        Stop,
        Pause,
        Continue,
        Restart
    }


    [Serializable]
    public struct ServiceCommand
    {
        public CommandType Command;
        public string ServiceName;
    }

    public enum ServiceControllerStatus
    {
        UndefinedStatus = 0,
        Stopped = 1,
        StartPending = 2,
        StopPending = 3,
        Running = 4,
        ContinuePending = 5,
        PausePending = 6,
        Paused = 7
    }


    #region Проверка\старт\стоп службы
    [Serializable]
    public struct ServiceStatus
    { 
        public ServiceControllerStatus Status;
        public string Name;
        public string Exception;
        public ServiceStatus(ServiceControllerStatus status, string name, string exception = null)
        {
            Status = status;
            Name = name;
            Exception = exception;
        }
    }
    #endregion
}
