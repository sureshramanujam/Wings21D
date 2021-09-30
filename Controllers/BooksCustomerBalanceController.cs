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
    public class BooksCustomerBalanceController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName, string custName)
        {
            if (string.IsNullOrEmpty(dbName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable PendingInvoices = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                con.Open();
                cmd.CommandText = "select * from Books_CustomersPendingBills_Table " +
                                 "Where CustomerName='" + custName + "' Order by BillDate, BillNumber";
                da.SelectCommand = cmd;
                PendingInvoices.TableName = "PendingInvoices";
                da.Fill(PendingInvoices);
                con.Close();
            }
            catch (Exception ex)
            {

            }

            var returnResponseObject = new
            {
                PendingInvoices = PendingInvoices
            };

            var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
            return response;
        }

        // GET api/values/5

        // POST api/values
        public HttpResponseMessage Post(List<BooksCustomerBalance> customerBalance)
        {
            var re = Request;
            var headers = re.Headers;

            string dbName = string.Empty;
            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }
            if (string.IsNullOrEmpty(dbName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            bool uploadAllData = false;
            if (headers.Contains("uploadall"))
            {
                string uploadAll = headers.GetValues("uploadall").First();
                uploadAllData = uploadAll.ToLower().Trim() == "true";
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            try
            {
                if (uploadAllData)
                {
                    con.Open();
                    cmd.CommandText = "Delete From Books_CustomersPendingBills_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                {
                    con.Open();
                    foreach (BooksCustomerBalance bcb in customerBalance)
                    {
                        bcb.customerName = bcb.customerName.Replace("'", "''");
                        cmd.CommandText = "Insert Into Books_CustomersPendingBills_Table Values('" + bcb.customerName + "', '"
                                          + bcb.billNumber + "', '" + bcb.billDate + "', " + bcb.pendingValue + ")";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}