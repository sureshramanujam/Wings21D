using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersSalesOrdersBooked
    {
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string DueDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int BookedQuantity { get; set; }
        public int LineAmount { get; set; }
        public string Username { get; set;  }
    }
}