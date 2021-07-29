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


namespace Wings21D.Controllers
{
    public class BooksCustomerCollectionController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string custName, string fromDate, string toDate)
        {
            if (String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(custName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            //DataSet ds = new DataSet();
            //List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Collection = new DataTable();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                string fromDt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                string toDt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");

                cmd.CommandText = "Select CustomerName,VoucherNumber, Convert(varchar,VoucherDate,23) as VoucherDate,PaymentMode," +
                    "ChequeNumber,AgainstInvoiceNumber,NetAmount,Username from Books_CustomersReceipts_Desktop_Table " +
                    "Where VoucherDate between '" + fromDt + "' and '" + toDt + "' and  CustomerName='" + custName + "' ";

                da.SelectCommand = cmd;
                Collection.TableName = "Collection";
                da.Fill(Collection);
                con.Close();
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            var returnResponseObject = new
            {
                Collection = Collection
            };

            var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
            return response;
        }
    }
}