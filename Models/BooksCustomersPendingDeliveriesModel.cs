using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersPendingDeliveries
    {
        public string party { get; set; }
        public string product { get; set; }
        public string batch { get; set; }
        public string dcno { get; set; }
        public string invoiceno { get; set; }
        public double billedqty { get; set; }
        public string branch { get; set; }
        public double lineamount { get; set; }
        public string userName { get; set;  }
    }
}