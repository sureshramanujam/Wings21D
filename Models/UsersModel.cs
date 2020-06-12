using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class Users
    {
        public string userName { get; set; }
        public string userPassword { get; set; }
        public int userStatus { get; set; }
        public int isRetailer { get; set; }
        public string customerName { get; set; }
    }
}