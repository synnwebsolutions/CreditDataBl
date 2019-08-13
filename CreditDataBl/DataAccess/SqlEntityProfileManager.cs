using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class SqlEntityProfileManager<TEntity> : SqlServerAccessBase where TEntity : class, new()
    {
        public SqlEntityProfileManager(string connStr)
        {
            base.OpenConnection(connStr);
        }

        string table;
        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(table))
                {
                    var obj = Activator.CreateInstance(typeof(TEntity));
                    var sqlEntAtrr = new GenericAttributeHelper<SqlEntityAttribute>().GetClassAttributes(obj).FirstOrDefault();
                    table = sqlEntAtrr.TableName;
                }
                return table;
            }
        }

        public void Insert(TEntity entity)
        {
            Dictionary<PropertyInfo, SqlFieldAttribute> fieldParams = new GenericAttributeHelper<SqlFieldAttribute>().GetPropertiesAttributes(entity);
            if (fieldParams.Any(x => x.Value.Identity))
            {
                ClearParams();
                var paramVals = new List<string>();
                foreach (var item in fieldParams)
                {
                    if (!item.Value.Identity)
                    {
                        var val = item.Key.GetValue(entity);
                        var parName = $"{parameterPrefix}{item.Key.Name}";
                        paramVals.Add(parName);
                        AddParam(parName, val, System.Data.ParameterDirection.Input, item.Value.FieldDbType);
                    }
                }
                string fields = string.Join(",", paramVals.Select(x => x.Replace(parameterPrefix, string.Empty)).ToList());
                string fieldsValuesKeys = string.Join(",", paramVals);
                _command.CommandText = $"INSERT INTO {TableName} ({fields}) VALUES({fieldsValuesKeys})";
                _command.ExecuteNonQuery();
            }
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            ExecuteNonQuery($"delete from { TableName } where id = { id.ToString()}");
        }

        public List<TEntity> Select(object searchParams)
        {
            throw new NotImplementedException();
        }

    }

}
