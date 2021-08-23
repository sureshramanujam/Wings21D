using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wings21D.Models
{
    public class BooksCustomerSaleReturnsDesktop
    {
        public string TransactionType { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceSubType { get; set; }
        public string AgainstInvoiceNumber { get; set; }
        public string AgainstInvoiceDate { get; set; }
        public decimal AgainstInvoiceValue { get; set; }
        public decimal InvoiceValue { get; set; }
        public string Remarks { get; set; }
        public string UserName { get; set; }
        public string BodyRowName { get; set; }
        public string AdjustmentAccountName { get; set; }
        public string ProductName { get; set; }
        public decimal InvoiceQuantity { get; set; }
        public string GSTRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal CessAmount { get; set; }
        public decimal LineAmount { get; set; }

    }
}