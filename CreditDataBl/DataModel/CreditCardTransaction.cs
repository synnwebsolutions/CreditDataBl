using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    [SqlEntity(TableName = "creditTransactions")]
    public class CreditCardTransaction
    {
        [SqlField(FieldDbType = System.Data.SqlDbType.Int, Identity = true)]
        public int TransactionId { get; set; }

        [DataNames("תאריך החיוב")]
        [SqlField(FieldDbType = System.Data.SqlDbType.DateTime)]
        public DateTime DebitDate { get; set; }

        [DataNames("תאריך העסקה")]
        [SqlField(FieldDbType = System.Data.SqlDbType.DateTime)]
        public DateTime TransactionDate { get; set; }

        [DataNames("שם בית העסק")]
        [SqlField(Nullable = true, FieldDbType = System.Data.SqlDbType.VarChar)]
        public string BussinessName { get; set; }

        [DataNames("סכום העסקה")]
        [SqlField(FieldDbType = System.Data.SqlDbType.Decimal)]
        public decimal TransactionOverallAmount { get; set; }

        [DataNames("סכום החיוב")]
        [SqlField(FieldDbType = System.Data.SqlDbType.Decimal)]
        public decimal TransactionDebitAmount { get; set; }

        [DataNames("פירוט נוסף")]
        [SqlField(FieldDbType = System.Data.SqlDbType.VarChar, Nullable = true)]
        public string TransactionDetails { get; set; }
        public bool Valid
        {
            get
            {
                return DebitDate != DateTime.MinValue || TransactionDate != DateTime.MinValue;
            }
        }

        public CreditCardTransaction()
        {

        }

        public int? BussinessId { get; set; }
    }
}
