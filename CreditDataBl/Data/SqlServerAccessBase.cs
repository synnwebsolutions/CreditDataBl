using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class SqlServerAccessBase
    {
        internal SqlConnection connection;
        internal SqlCommand _command;
        internal SqlTransaction transaction;
        internal const string parameterPrefix = "@";
        //private string appDBConnStr()
        //{
        //    //var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
        //    //IConfiguration Configuration = builder.Build();
        //    return Configuration["ConnectionStrings:sluggerConnection"];
        //}


        public void OpenConnection(string ConnectionString)
        {
            try
            {
                this.connection = new SqlConnection();
                this.connection.ConnectionString = ConnectionString;
                this.connection.Open();
                this._command = new SqlCommand();
                this._command.Connection = this.connection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CloseConnection()
        {
            try
            {
                this.connection.Close();
            }
            catch
            {

            }
        }

        public void TransactionBegin()
        {
            this.transaction = this.connection.BeginTransaction();
        }

        public void TransactionCommit()
        {
            this.transaction.Commit();
        }

        public void TransactionRollback()
        {
            this.transaction.Rollback();
        }

        public void ClearParams()
        {
            this._command.Parameters.Clear();
        }

        public object GetParamValue(string Name)
        {
            return this._command.Parameters[Name].Value;
        }

        public SqlParameter GetParam(string Name)
        {
            return this._command.Parameters[Name];
        }

        public void AddParam(string Name, object Value, ParameterDirection Direction, SqlDbType Type)
        {
            //this._command.Parameters.Add(Name, Type);
            //this._command.Parameters[Name].Direction = Direction;

            //if(Value == null)
            //    this._command.Parameters[Name].Value = DBNull.Value;
            //else
            //    this._command.Parameters[Name].Value = Value;
            this._command.Parameters.AddWithValue(Name, Value ?? DBNull.Value);
        }

        public void CallProcedure(string ProcedureName)
        {
            try
            {
                this._command.CommandText = ProcedureName;
                this._command.CommandType = CommandType.StoredProcedure;
                this._command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ExecuteNonQuery(string SQL)
        {
            try
            {
                DataTable dataTable = new DataTable();
                SqlCommand sqlCommand = new SqlCommand(SQL, this.connection);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExecuteReader(string SQL)
        {
            DataTable result;
            try
            {
                DataTable dataTable = new DataTable();
                this._command = new SqlCommand(SQL, this.connection);
                SqlDataReader reader = this._command.ExecuteReader();
                dataTable.Load(reader);
                bool flag = dataTable.Rows.Count > 0;
                if (flag)
                {
                    result = dataTable;
                }
                else
                {
                    result = new DataTable();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public object GetSingleValue(string SQL)
        {
            object result;
            try
            {
                DataTable dataTable = new DataTable();
                SqlCommand sqlCommand = new SqlCommand(SQL, this.connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                dataTable.Load(reader);
                bool flag = dataTable.Rows.Count > 0;
                if (flag)
                {
                    result = dataTable.Rows[0][0];
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool hasResults(string table, string condition)
        {
            string sql = "select count(*) from " + table + " where " + condition;
            string count = GetSingleValue(sql).ToString();
            return int.Parse(count) > 0;
        }

        public int getNextSeq(string sequence)
        {
            string sql = "select NEXT VALUE FOR " + sequence;
            string seq = GetSingleValue(sql).ToString();
            return int.Parse(seq);
        }
    }

}
