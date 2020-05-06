using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersDeliveries
    {
        public string orderno { get; set; }
        public string dcno { get; set; }
        public string date { get; set; }
        public string party { get; set; }
        public string product { get; set; }
        public double qty { get; set; }
        public double lineamount { get; set; }
        public string userName { get; set;  }
    }
}