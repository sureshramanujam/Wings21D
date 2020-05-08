using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersPayments
    {
        public string CustomerName { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherDate { get; set; }
        public string PaymentMode { get; set; }
        public string ChequeNumber { get; set; }
        public string AgainstInvoiceNumber { get; set; }
        public double NetAmount { get; set; }
        public string Username { get; set;  }
    }
}