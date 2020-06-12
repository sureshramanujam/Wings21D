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
        public string BillingAddressLineOne { get; set; }
        public string BillingAddressLineTwo { get; set; }
        public string BillingAddressLineThree { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressCountry { get; set; }
        public string BillingAddressPincode { get; set; }
        public string BillingAddressContactPerson { get; set; }
        public string ShippingAddressLineOne { get; set; }
        public string ShippingAddressLineTwo { get; set; }
        public string ShippingAddressLineThree { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShippingAddressCountry { get; set; }
        public string ShippingPincode { get; set; }
        public string ShippingAddressContactPerson { get; set; }
        public string PersonalPhone { get; set; }
        public string OfficePhone { get; set; }
        public string EmailId { get; set; }
    }
}