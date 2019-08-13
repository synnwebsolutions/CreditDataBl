using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SqlEntityAttribute : Attribute
    {
        public string TableName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SqlFieldAttribute : Attribute
    {
        public string FieldName { get; set; }
        public SqlDbType FieldDbType { get; set; }
        public bool Identity { get; set; }
    }
}
