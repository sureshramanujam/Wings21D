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
    public class TradeCustomerBalanceController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName, string custName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable PendingInvoices = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(custName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "select * from Trade_CustomerPendingBills_Table " +
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
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        // GET api/values/5

        // POST api/values
        public HttpResponseMessage Post(List<TradeCustomerBalance> customerBalance)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            String uploadAll = String.Empty;

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            if (headers.Contains("uploadall"))
            {
                uploadAll = headers.GetValues("uploadall").First();
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            /*
            con.Open();
            cmd.CommandText = "Select * From Trade_CustomerPendingBills_Table";
            SqlDataAdapter customerBalancesAdapter = new SqlDataAdapter();
            DataTable availableCustoemrBalances = new DataTable();
            customerBalancesAdapter.SelectCommand = cmd;
            customerBalancesAdapter.Fill(availableCustoemrBalances);
            con.Close();
            */

            if (!String.IsNullOrEmpty(dbName))
            {
                if (uploadAll.ToLower().Trim() == "true")
                {
                    try
                    {
                        //if (availableCustoemrBalances.Rows.Count > 0)
                        //{//}
                        con.Open();
                        cmd.CommandText = "Delete from Trade_CustomerPendingBills_Table";
                        cmd.ExecuteNonQuery();

                        con.Open();

                        foreach (TradeCustomerBalance tcb in customerBalance)
                        {
                            tcb.customerName = tcb.customerName.Replace("'", "''");
                            cmd.CommandText = "Insert Into Trade_CustomerPendingBills_Table Values('" + tcb.customerName + "', '"
                                              + tcb.billNumber + "', '" + tcb.billDate + "', " + tcb.pendingValue + ")";
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
                    con.Open();
                    foreach (TradeCustomerBalance tcb in customerBalance)
                    {
                        tcb.customerName = tcb.customerName.Replace("'", "''");
                        cmd.CommandText = "Insert Into Trade_CustomerPendingBills_Table Values('" + tcb.customerName + "', '"
                                          + tcb.billNumber + "', '" + tcb.billDate + "', " + tcb.pendingValue + ")";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                    return new HttpResponseMessage(HttpStatusCode.Created);
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
