using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Wings21D.Controllers
{
    public class TradeExecutiveDashboardController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName, string userName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();

            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Dashboard = new DataTable();
            DataTable UserIDTable = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(userName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Select UserName from CompanyUsers_Table Where UserName='" + userName + "'";
                    cmd.Connection = con;
                    da.SelectCommand = cmd;
                    da.Fill(UserIDTable);

                    cmd.CommandText = "SELECT " +
                                      "(SELECT count(*) From Collections_Table where Username='" + UserIDTable.Rows[0][0].ToString() + "') AS CollectionCount, " +
                                      "(SELECT sum(cashamount)+sum(chequeamount) From Collections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "') AS CollectionAmount, " +
                                      "(SELECT count(*) From CashCollections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "') AS CashTransactions, " +
                                      "(SELECT sum(amount) From CashCollections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "') AS CashAmount, " +
                                      "(SELECT count(*) From ChequeCollections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "') AS ChequeTransactions, " +
                                      "(SELECT sum(amount) From ChequeCollections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "') AS ChequeAmount";

                    da.SelectCommand = cmd;
                    Dashboard.TableName = "Dashboard";
                    da.Fill(Dashboard);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Dashboard = Dashboard
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }


        [HttpGet]
        public string GetSOSummary(string dbName, string userName)
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
                    //DateTime asonDate = DateTime.Parse(asAtDate);

                    cmd.CommandText = "With SalesOrdersList As (" +
                                        "Select a.DocumentNo,  Format(TransactionDate, 'yyyy-MM-dd') As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces * b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks * b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName = b.ItemName " +
                                      ")Select Sum(AmtPcs+AmtPacks) As 'OrderValue' From SalesOrdersList Where Username = 'Supervisor'";

                    da.SelectCommand = cmd;
                    SalesOrders.TableName = "SalesOrders";
                    da.Fill(SalesOrders);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return "Connection error.";
                }

                return SalesOrders.Rows[0][0].ToString();
            }
            else
            {
                return "Database not found.";
            }
        }


        // GET api/values/5

        // POST api/values
        public void Post([FromBody]string value)
        {
        }
    }
}
