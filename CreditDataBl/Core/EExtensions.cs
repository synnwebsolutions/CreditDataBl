using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public static class PropertyMapExtensions
    {
        public static DateTime MapDateTime(this string strDate)
        {
            var dt = DateTime.MinValue;

            try
            {
                var val = strDate.Split('/');
                var year = int.Parse(val[2]);
                var month = int.Parse(val[1]);
                var day = int.Parse(val[0]);

                dt = new DateTime(year, month, day);
            }
            catch (Exception ex)
            {
                string m = ex.Message;
            }

            return dt;
        }

        public static bool IsXLS(this string path)
        {
            return Path.GetExtension(path) == ".xls";
        }
    }
}
