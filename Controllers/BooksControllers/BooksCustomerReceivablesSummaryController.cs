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

namespace Wings21D.Controllers.BooksControllers
{
    public class BooksCustomerReceivablesSummaryController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get(string dbName)
        {
            if (String.IsNullOrEmpty(dbName))
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
                cmd.CommandText = "Select DISTINCT a.CustomerName, ISNULL(a.CustomerCity,'Not Set') CustomerCity, " +
                                  "ISNULL(a.GSTNumber,'Not Set') GSTNumber,Count(BillNumber) as BillNumber, ISNULL(Sum(b.PendingValue),0) TotalDue " +
                                  "From Books_Customers_Table a LEFT Join Books_CustomersPendingBills_Table b " +
                                  "On a.CustomerName = b.CustomerName " +
                                  "Group by a. CustomerName, b.CustomerName, a.GSTNumber, a.CustomerCity " +
                                  "Order by a.CustomerName";


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

    }
}