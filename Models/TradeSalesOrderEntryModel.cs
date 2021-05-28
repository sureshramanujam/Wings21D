using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class SalesOrderEntry
    {
        public string transactionDate { get; set; }
        public string customerName { get; set; }
        public string itemName { get; set; }
        public int quantityInPieces { get; set; }
        public int quantityInPacks { get; set; }
        public string transactionRemarks { get; set; }
        public string userName { get; set; }
        public string profitCenterName { get; set; }
        public string beatName { get; set; }
    }
}