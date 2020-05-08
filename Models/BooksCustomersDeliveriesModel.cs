using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersDeliveries
    {
        public string OrderNumber { get; set; }
        public string DCNumber { get; set; }
        public string DCDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public double LineAmount { get; set; }
        public string Username { get; set;  }
    }
}