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
    public class BooksCustomerReceivablesDetailedController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string custName)
        {
            if (String.IsNullOrEmpty(dbName) && String.IsNullOrEmpty(custName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable CustomerReceivable = new DataTable();

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = "Select CustomerName,BillNumber,Convert(varchar,BillDate,105) As BillDate,PendingValue " +
                                  " from Books_CustomersPendingBills_Table  Where CustomerName='" + custName + "' ";


                da.SelectCommand = cmd;
                CustomerReceivable.TableName = "CustomerLedger";
                da.Fill(CustomerReceivable);
                con.Close();
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            var returnResponseObject = new
            {
                CustomerReceivable = CustomerReceivable
            };

            var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
            return response;
        }

    }

}
