using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wings21D.Models
{
    public class BooksCustomerCreditNotes 
    {
        public string TransactionType { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public string AgainstInvoiceNumber { get; set; }
        public string AgainstInvoiceDate { get; set; }
        public double AgainstInvoiceValue { get; set; }
        public string AdjustmentAccountName { get; set; }
        public string ProductName { get; set; }
        public double InvoiceQuantity { get; set; }
        public double InvoiceValue { get; set; }
        public string BodyRowName { get; set; }
        public string GSTRate { get; set; }
        public double DiscountAmount { get; set; }
        public double TaxableValue { get; set; }
        public double GSTAmount { get; set; }
        public double CessAmount { get; set; }
        public double LineAmount { get; set; }
        public string Remarks { get; set; }
        public string UserName { get; set; }
    }
}