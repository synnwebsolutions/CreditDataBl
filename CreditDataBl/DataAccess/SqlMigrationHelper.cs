using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public static class SqlMigrationHelper
    {
        public static string GenerateSqlCreateFildScript(PropertyInfo info, SqlFieldAttribute sqlField)
        {
            //[Id] [int] IDENTITY(1,1) NOT NULL,
            var identityText = sqlField.Identity ? "IDENTITY(1,1)" : string.Empty;
            var nullState = "NULL";
            if (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                || info.PropertyType == typeof(string))
                nullState = "NOT " + nullState;
            return $"[{sqlField.FieldName}] [{ sqlField.FieldDbType.ToString()}] {identityText} {nullState}";
        }
    }
}
