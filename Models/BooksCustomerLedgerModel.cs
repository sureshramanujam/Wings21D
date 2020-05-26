using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomerLedger
    {
        public string TransactionType { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherDate { get; set; }
        public string Account { get; set; }
        public string ContraAccount { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
        public double BalanceAmount { get; set; }
        public string Remarks { get; set; }
    }
}