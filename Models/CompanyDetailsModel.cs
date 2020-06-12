using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wings21D.Models
{
    public class CompanyDetails
    {   
        public string CompanyDatabaseName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLegalName { get; set; }
        public string CompanyTradeName { get; set; }
        public string CompanyAddressLine1 { get; set; }
        public string CompanyAddressLine2 { get; set; }
        public string CompanyAddressLine3 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyPINCode { get; set; }
        public string CompanyFYStartDate { get; set; }
        public string CompanyFYEndDate { get; set; }
    }
}