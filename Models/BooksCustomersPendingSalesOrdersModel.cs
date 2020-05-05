using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersPendingSalesOrders
    {
        public string orderno { get; set; }
        public string date { get; set; }
        public string duedate { get; set; }
        public string party { get; set; }
        public string product { get; set; }
        public int pendingqty { get; set; }
        public int lineamount { get; set; }
        public string branch { get; set; }
        public string userName { get; set;  }
    }
}