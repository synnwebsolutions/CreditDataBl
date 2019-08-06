using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class DataNamesMapper<TEntity> where TEntity : class, new()
    {
        public TEntity Map(DataRow row)
        {
            //Step 1 - Get the Column Names
            var columnNames = row.Table.Columns
                                       .Cast<DataColumn>()
                                       .Select(x => x.ColumnName)
                                       .ToList();

            //Step 2 - Get the Property Data Names
            var properties = (typeof(TEntity)).GetProperties()
                                              .Where(x => x.GetCustomAttributes(typeof(DataNamesAttribute), true).Any())
                                              .ToList();

            //Step 3 - Map the data
            TEntity entity = new TEntity();
            foreach (var prop in properties)
            {
                PropertyMapHelper.Map(typeof(TEntity), row, prop, entity);
            }

            return entity;
        }

        public IEnumerable<TEntity> Map(DataTable table)
        {
            //Step 1 - Get the Column Names
            var tableColumnNames = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            //Step 2 - Get the Property Data Names
            var properties = (typeof(TEntity)).GetProperties()
                                                .Where(x => x.GetCustomAttributes(typeof(DataNamesAttribute), true).Any())
                                                .ToList();

            PropertyMapHelper.ValidateColumnsMatch(typeof(TEntity), table, properties);

            //Step 3 - Map the data
            List<TEntity> entities = new List<TEntity>();
            foreach (DataRow row in table.Rows)
            {
                TEntity entity = new TEntity();
                foreach (var prop in properties)
                {
                    PropertyMapHelper.Map(typeof(TEntity), row, prop, entity);
                }
                entities.Add(entity);
            }

            return entities;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DataNamesAttribute : Attribute
    {
        protected List<string> _valueNames { get; set; }

        public List<string> ValueNames
        {
            get
            {
                return _valueNames;
            }
            set
            {
                _valueNames = value;
            }
        }

        public DataNamesAttribute()
        {
            _valueNames = new List<string>();
        }

        public DataNamesAttribute(params string[] valueNames)
        {
            _valueNames = valueNames.ToList();
        }
    }

    public class AttributeHelper
    {
        public static List<string> GetDataNames(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName).GetCustomAttributes(false).Where(x => x.GetType().Name == "DataNamesAttribute").FirstOrDefault();
            if (property != null)
            {
                return ((DataNamesAttribute)property).ValueNames;
            }
            return new List<string>();
        }

        public static string GetDataName(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName).GetCustomAttributes(false).Where(x => x.GetType().Name == "DataNamesAttribute").FirstOrDefault();
            if (property != null)
            {
                return ((DataNamesAttribute)property).ValueNames.FirstOrDefault();
            }
            return string.Empty;
        }
    }

    public class PropertyMapHelper
    {
        public static void Map(Type type, DataRow row, PropertyInfo prop, object entity)
        {
            List<string> columnNames = AttributeHelper.GetDataNames(type, prop.Name);

            foreach (var columnName in columnNames)
            {
                if (!String.IsNullOrWhiteSpace(columnName) && row.Table.Columns.Contains(columnName))
                {
                    var propertyValue = row[columnName];
                    if (propertyValue != DBNull.Value)
                    {
                        ParsePrimitive(prop, entity, row[columnName]);
                        break;
                    }
                }
            }
        }

        private static void ParsePrimitive(PropertyInfo prop, object entity, object value)
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(entity, value.ToString().Trim(), null);
            }
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
            {
                if (value == null)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    prop.SetValue(entity, int.Parse(value.ToString()), null);
                }
            }

            else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
            {
                if (value == null)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    try
                    {
                        prop.SetValue(entity, decimal.Parse(value.ToString(), NumberStyles.Currency, CultureInfo.GetCultureInfo("he-IL")), null);
                    }
                    catch (Exception ex)
                    {
                        prop.SetValue(entity, decimal.Parse(value.ToString(), NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US")), null);
                    }
                }
            }

            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                if (value == null)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    try
                    {
                        prop.SetValue(entity, DateTime.Parse(value.ToString()), null);
                    }
                    catch (Exception ex)
                    {
                        var val = value.ToString().MapDateTime();
                        prop.SetValue(entity, val, null);
                    }
                }
            }
        }

        internal static List<string> ValidateColumnsMatch(Type type, DataTable table, List<PropertyInfo> properties)
        {
            List<string> columnNames = new List<string>();
            foreach (PropertyInfo prop in properties)
                columnNames.Add(AttributeHelper.GetDataName(type, prop.Name));

            List<DataRow> matchingRows = new List<DataRow>();

            foreach (var columnName in columnNames)
            {
                // scan each rows
                foreach (DataRow row in table.Rows)
                {
                    if (row.ItemArray.Any(x => x.ToString() == columnName))
                    {
                        if (!matchingRows.Contains(row))
                        {
                            matchingRows.Add(row);
                        }
                    }
                }
            }

            return columnNames;
        }
    }

}
