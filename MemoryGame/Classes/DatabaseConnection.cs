using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace Windowss.Classes
{
    internal class DatabaseConnection
    {
        private static readonly object key = new object();
        public SqlConnection Connection;
        private DatabaseConnection()
        {
            Connection = new SqlConnection(@"Data Source=(localdb)\instanceofgame;Initial Catalog=MemoryGame;Integrated Security=True");
        }

        private static DatabaseConnection instance = null;

        public static DatabaseConnection Instance
        {
            get
            {
                if (instance == null)
                    lock (key)
                        if (instance == null)
                            instance = new DatabaseConnection();

                return instance;
            }
        }



    }
}