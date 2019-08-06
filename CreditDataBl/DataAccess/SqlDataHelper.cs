using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl.DataAccess
{
    public class SqlDataHelper
    {
        private static SqlDataHelper db;
        private string connectionString;
        private string tableName;

        public SqlDataHelper(string connectionString, string tableName)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;
        }

        public static void Init(string connectionString, string tableName)
        {
            db = new SqlDataHelper(connectionString, tableName);
        }

        public static SqlDataHelper Instance
        {
            get
            {
                if (db == null)
                    throw new Exception("Db Helper Not Initialized !");
                return db;
            }
        }



    }
}
