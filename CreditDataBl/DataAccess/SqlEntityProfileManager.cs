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

        public Dictionary<PropertyInfo, SqlFieldAttribute> FieldsParameters
        {
            get
            {
                return new GenericAttributeHelper<SqlFieldAttribute>().GetPropertiesAttributes(Activator.CreateInstance(typeof(TEntity)));
            }
        }

        public List<string> ParametersWithPrefixNames
        {
            get
            {
                var parametersWithPrefixNames = new List<string>();
                foreach (var item in FieldsParameters)
                {
                    if (!item.Value.Identity)
                    {
                        var parName = $"{parameterPrefix}{item.Key.Name}";
                        parametersWithPrefixNames.Add(parName);
                    }
                }
                return parametersWithPrefixNames;
            }
        }

        public void Insert(TEntity entity)
        {
            ClearParams();

            GenerateInsertUpdateParams(entity, false);

            string fields = string.Join(",", ParametersWithPrefixNames.Select(x => x.Replace(parameterPrefix, string.Empty)).ToList());
            _command.CommandText = $"INSERT INTO {TableName} ({fields}) VALUES({string.Join(",", ParametersWithPrefixNames)})";
            _command.ExecuteNonQuery();
        }

        private void GenerateInsertUpdateParams(TEntity entity, bool isUpdate)
        {
            foreach (var item in FieldsParameters)
            {
                if (!item.Value.Identity || isUpdate)
                {
                    var val = item.Key.GetValue(entity);
                    var parName = $"{parameterPrefix}{item.Key.Name}";
                    AddParam(parName, val, System.Data.ParameterDirection.Input, item.Value.FieldDbType);
                }
            }
        }

        public void Update(TEntity entity)
        {
            ClearParams();

            GenerateInsertUpdateParams(entity, true);
            List<string> elements = new List<string>();
            string idElement = FieldsParameters.Where(x => x.Value.Identity).Select(x => $"[{x.Key.Name}] = {parameterPrefix}{x.Key.Name}").FirstOrDefault();

            foreach (var parametersWithPrefix in ParametersWithPrefixNames)
            {
                elements.Add($"[{parametersWithPrefix.Remove(0, 1)}] = {parametersWithPrefix}");
            }

            _command.CommandText = $"UPDATE {TableName} SET {string.Join(",", elements)} WHERE {idElement} ";
            _command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            ExecuteNonQuery($"delete from { TableName } where id = { id.ToString()}");
        }

        public List<TEntity> Select(object searchParams)
        {
            ClearParams();
            var mapper = new DataNamesMapper<TEntity>();
            List<TEntity> lst = null;
            _command.CommandText = $"select * from {TableName}";
            using (SqlDataReader dr = _command.ExecuteReader())
            {
                var tbl = new DataTable();
                tbl.Load(dr);
                lst = mapper.Map(tbl).ToList();
            }
            return lst;
        }
    }

}
