using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace TelegramBot
{
    class DB
    {
        public SqlConnection connection;

        public DB()
        {
            connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TeleDB;Integrated Security=True");
        }

        public void OpenConnection()
        {
            Console.WriteLine("Opening connection");
            connection.Open();
        }
    }
}
