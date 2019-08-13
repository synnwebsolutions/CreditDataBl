using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class DbMigrationHandler
    {
        public static void MigrateTable(Type sqlEntity)
        {
            var obj = Activator.CreateInstance(sqlEntity);
            var sqlEntAtrr = new GenericAttributeHelper<SqlEntityAttribute>().GetClassAttributes(obj).FirstOrDefault();
            var table = sqlEntAtrr.TableName;

            Dictionary<PropertyInfo, SqlFieldAttribute> fieldParams = new GenericAttributeHelper<SqlFieldAttribute>().GetPropertiesAttributes(obj);

        }
    }
}
