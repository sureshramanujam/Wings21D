using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksSalesInvoiceEntryPayments
    {
        public string TransactionNumber { get; set; }
        public double CashAmount { get; set; }
        public double ChequeAmount { get; set; }
        public double ChequeNumber { get; set; }
        public string ChequeDate { get; set; }
        public string Username { get; set; }
    }
}