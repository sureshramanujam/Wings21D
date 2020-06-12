using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksSalesInvoiceEntry
    {
        public string TransactionDate { get; set; }
        public string InvoiceType { get; set; }

        public string CashCreditType { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        //public double CGSTAmount { get; set; }
        //public double SGSTAmount { get; set; }
        //public double IGSTAmount { get; set; }
        //public double LineAmount { get; set; }
        public string TransactionRemarks { get; set; }
        //public string BranchName { get; set; }
        //public string LocationName { get; set; }
        //public string DivisionName { get; set; }
        //public string ProjectName { get; set; }
        public string UserName { get; set; }
    }
}