using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace Network
{
    public class MySQL
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "192.168.0.1";
            int port = 3306;
            string database = "prison";
            string username = "root";
            string password = "CtHDbCGK.C";

            return CreateDBConnection(host, port, database, username, password);
        }
        public static MySqlConnection CreateDBConnection(string host, int port, string database, string username, string password)
        {
            String connString = "Server=" + host + ";Database=" + database
                + ";port=" + port + ";User Id=" + username + ";password=" + password + ";AllowZeroDateTime=true";

            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }
        public static MySqlDataReader GetDataReader(string sql)
        {
            MySqlConnection connection = MySQL.GetDBConnection();
            connection.Open();

            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();
            //connection.Close();
            return reader;
        }
    }
}

