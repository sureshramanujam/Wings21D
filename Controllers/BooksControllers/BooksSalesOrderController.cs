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
    public class BooksSalesOrderController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string asAtDate)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesOrders = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    DateTime asonDate = DateTime.Parse(asAtDate);

                    cmd.CommandText = "select a.DocumentNo, Convert(varchar,a.TransactionDate,23) as TransactionDate, a.CustomerName," +
                                      "a.ProductName, a.Quantity, a.TransactionRemarks, a.Username from Books_SalesOrder_Table a Where " +
                                      "convert(varchar,a.TransactionDate,23) <= '" + asonDate.ToString() + "' And a.DownloadedFlag=0 Order By a.DocumentNo";
                    da.SelectCommand = cmd;
                    SalesOrders.TableName = "SalesOrders";
                    da.Fill(SalesOrders);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    SalesOrders = SalesOrders
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
                
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>                
        [HttpPost]
        public HttpResponseMessage SaveOrder(List<BooksSalesOrderEntry> mySO)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            DateTime todayDate = DateTime.Now;
            var dateOnly = todayDate.Date;

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    //cmd.CommandText = "Select ISNULL(Max(TransactionNo), 0) + 1 From Trade_SalesOrder_Table Where YEAR(convert(varchar,TransactionDate,23)) = '" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "'";
                    cmd.CommandText = "Select ISNULL(Max(TransactionNo), 0) + 1 From Books_SalesOrder_Table";
                    SqlDataAdapter docNumberAdapter = new SqlDataAdapter();
                    DataTable newDocumentNumber = new DataTable();
                    docNumberAdapter.SelectCommand = cmd;
                    docNumberAdapter.Fill(newDocumentNumber);

                    string transRemarks = String.Empty;
                        
                        

                    foreach (BooksSalesOrderEntry soe in mySO)
                    {
                        transRemarks = soe.transactionRemarks.Replace("\\n", "");
                        soe.customerName = soe.customerName.Replace("'", "''");
                        soe.productName = soe.productName.Replace("'", "''");

                        cmd.CommandText = "Insert Into Books_SalesOrder_Table Values(" + Convert.ToInt32(newDocumentNumber.Rows[0][0]) +
                                          ",'" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "','" + soe.customerName + "', '" + soe.productName + "'," +
                                          soe.quantity + ",'" + transRemarks + "','GSO-M-'," +
                                          "'GSO-M-" + Convert.ToInt32(newDocumentNumber.Rows[0][0]).ToString() +  "',0,'" + soe.userName + "')";

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
