using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class SqlMigrationHandler<TEntity> where TEntity : class, new()
    {
        private string _connectionString;
        public SqlMigrationHandler(string connectionString)
        {
            _connectionString = connectionString;
        }
        public Type EntityType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        public object Entity
        {
            get
            {
                return Activator.CreateInstance(EntityType);
            }
        }

        public string EntityTable
        {
            get
            {
                return new GenericAttributeHelper<SqlEntityAttribute>().GetClassAttributes(Entity).FirstOrDefault().TableName;
            }
        }

        public void MigrateTable()
        {
            Dictionary<PropertyInfo, SqlFieldAttribute> fieldParams = new GenericAttributeHelper<SqlFieldAttribute>().GetPropertiesAttributes(Entity);
            List<string> fieldsScripts = new List<string>();
            foreach (var item in fieldParams.OrderBy(x => x.Value.Identity).ToList())
                fieldsScripts.Add(item.Value.GenerateCreateSqlFildScript(item.Key, EntityTable));

            var createTabeScript = $" CREATE TABLE [dbo].[{EntityTable}] ({string.Join(",", fieldsScripts) }) ON [PRIMARY]";
            if (!EntityTableExists())
                ExecuteCreateTable(createTabeScript);
        }

        private void ExecuteCreateTable(string createTabeScript)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(createTabeScript, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private bool EntityTableExists()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = @table) SELECT 1 ELSE SELECT 0";

                conn.Open();
                cmd.Parameters.Add("@table", SqlDbType.NVarChar).Value = EntityTable;
                return (int)cmd.ExecuteScalar() == 1;
            }
        }

    }

    public static class MigrationExtensions
    {
        public static string GenerateCreateSqlFildScript(this SqlFieldAttribute sqlField, PropertyInfo prop, string entityTable)
        {
            var fieldName = sqlField.FieldName ?? prop.Name;
            string fieldType = sqlField.FieldDbType.ClrType();

            if (sqlField.Identity)
                return GenerateCreateSqlIdentityFildScript(fieldName, fieldType, entityTable);

            var nullState = sqlField.Nullable ? "NULL" : "NOT NULL";
            return $"[{fieldName}] [{ fieldType }]{sqlField.FieldDbType.ClrLenght()} {nullState}";
        }

        private static string GenerateCreateSqlIdentityFildScript(string fieldName, string fieldType, string entityTable)
        {
            return $@"[{fieldName}] [{ fieldType }] IDENTITY(1,1) NOT NULL,	
                      CONSTRAINT[PK_{entityTable}] PRIMARY KEY CLUSTERED([{fieldName}] ASC)
	                   WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]";
        }

        public static string ClrType(this SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.Bit:
                    return "bit";
                case SqlDbType.DateTime:
                    return "datetime";
                case SqlDbType.Decimal:
                    return "decimal";
                case SqlDbType.Int:
                    return "int";
                case SqlDbType.VarChar:
                    return "varchar";
                case SqlDbType.Date:
                    return "datetime";
                case SqlDbType.Time:
                    return "date";
                default:
                    break;
            }
            return string.Empty;
        }

        public static string ClrLenght(this SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.Decimal:
                    return "(18,0)";
                case SqlDbType.VarChar:
                    return "(max)";
                default:
                    break;
            }
            return string.Empty;
        }
    }

}
