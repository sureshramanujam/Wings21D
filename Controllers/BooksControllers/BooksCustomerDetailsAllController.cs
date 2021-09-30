using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Wings21D.Models;
using System.Linq;
using System.Web.Script.Serialization;

namespace Wings21D.Controllers
{
    public class BooksCustomerDetailsAllController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get(string dbName ,string custName)
        {
            if (String.IsNullOrEmpty(dbName)|| String.IsNullOrEmpty(custName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Customers = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = con;
                con.Open();
                
                cmd.CommandText = "Select a.CustomerName,ISNULL(a.GSTNumber, '') As GSTNumber,ISNULL(a.CustomerCity, '') As CustomerCity," +
                                  "ISNULL(b.AddressLine1, '') As ShippingAddress, ISNULL(b.City, '') As ShippingCity, ISNULL(b.ContactPerson, '') As ShippingContact, " +
                                  "ISNULL(c.AddressLine1, '') As OfficeAddress,ISNULL(c.City, '') As OfficeCity, ISNULL(c.State, '') As OfficeState, ISNULL(c.ContactPerson, '') As OfficeContact "+
                                  "From Books_Customers_Table a "+
                                  "Left Join Books_CustomerShippingAddress_Table b on b.CustomerName = a.CustomerName "+
                                  "Left Join Books_CustomerOfficeAddress_Table c on a.CustomerName = c.CustomerName "+
                                  "Where a.CustomerName = '" + custName + "'";

                da.SelectCommand = cmd;
                Customers.TableName = "Customers";
                da.Fill(Customers);
                con.Close();
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            var returnResponseObject = new
            {
                Customers = Customers
            };

            var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
            return response;
        }

        // GET api/values/5

       
       
    }
}