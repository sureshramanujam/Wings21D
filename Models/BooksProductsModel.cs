using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksProducts
    {
        public string productName   { get; set; }
        public string hsnsac        { get; set; }
        public double rateperpiece { get; set; }
        public string gstrate       { get; set; }
        public double productmrp    { get; set; }
        public int activeStatus     { get; set; }
    }
}