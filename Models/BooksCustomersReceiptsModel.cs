using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomersReceipts
    {
        public string account { get; set; }
        public string voucherno{ get; set; }
        public string voucherdate { get; set; }
        public string paymentmode { get; set; }
        public string chequeno { get; set; }
        public string againstinvno { get; set; }
        public double netamount{ get; set; }
        public string userName { get; set;  }
    }
}