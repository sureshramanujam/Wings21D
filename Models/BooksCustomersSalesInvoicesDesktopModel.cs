using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersSalesInvoicesDesktop
    {
        public string TransactionType { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceSubType { get; set; }
        public string EWayBillNumber { get; set; }
        public string ProductName { get; set; }
        public double InvoiceQuantity { get; set; }
        public string GSTRate { get; set; }
        public double DiscountAmount { get; set; }
        public double GSTAmount { get; set; }
        public double CessAmount { get; set; }
        public double LineAmount { get; set; }
        public string OrderNumber { get; set; }
        public string DeliveryNumber { get; set; }
        public string Remarks { get; set; }
        public string Username { get; set;  }
    }
}