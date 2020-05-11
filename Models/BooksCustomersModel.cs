using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class BooksCustomers
    {
        public string CustomerName { get; set; }
        public string GSTNumber { get; set; }
        public string City { get; set; }
        public int ActiveStatus { get; set; }
        public string OfficeAddressLine1 { get; set; }
        public string OfficeAddressLine2 { get; set; }
        public string OfficeAddressLine3 { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeState { get; set; }
        public string OfficeCountry { get; set; }
        public string OfficePINCode { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingAddressLine3 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingPINCode { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string AlternateContactnumber { get; set; }
        public string EmailId { get; set; }
    }
}