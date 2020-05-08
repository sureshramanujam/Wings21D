using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersPendingDeliveries
    {
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string BatchName { get; set; }
        public string DCNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public double BilledQuantity { get; set; }
        public string BranchName { get; set; }
        public double LineAmount { get; set; }
        public string Username { get; set;  }
    }
}