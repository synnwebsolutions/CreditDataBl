using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class CreditCardTransaction
    {
        public DateTime TransactionDate { get; set; }
        public string BussinessName { get; set; }
        public decimal OverallAmount { get; set; }
        public decimal Amount { get; set; }
        public string TransactionDetails { get; set; }

        public int? BussinessId { get; set; }
    }
}
