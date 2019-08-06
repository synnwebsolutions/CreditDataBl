using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class CreditCardTransaction
    {
        [DataNames("תאריך החיוב")]
        public DateTime DebitDate { get; set; }

        [DataNames("תאריך העסקה")]
        public DateTime TransactionDate { get; set; }

        [DataNames("שם בית העסק")]
        public string BussinessName { get; set; }

        [DataNames("סכום העסקה")]
        public decimal TransactionOverallAmount { get; set; }

        [DataNames("סכום החיוב")]
        public decimal TransactionDebitAmount { get; set; }

        [DataNames("פירוט נוסף")]
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
