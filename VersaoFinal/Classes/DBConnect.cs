using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaoFinal.Classes
{
    class DBConnect
    {
        private SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-6VEFE45;Initial Catalog=smarketdb;User ID=sa;Password=197325");
        public SqlConnection GetCon()
        {
            return connection;
        }
        public void OpenCon()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }
        public void CloseCon()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}