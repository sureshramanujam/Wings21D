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
    public class TradeSalesOrdersReportController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string fromDate, string toDate, string userName)
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
                    string fromDateString = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                    string toDateString = DateTime.Parse(toDate).ToString("yyyy-MM-dd");

                    cmd.CommandText = "With SalesOrdersList As( " +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces*b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks*b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName=b.ItemName " +
                                      ") " +
                                      "Select DocumentNo,Convert(varchar,OrderDate,105) OrderDate, CustomerName, Sum(QuantityInPieces+QuantityInPacks) As 'TotalQty', " +
                                      "Sum(AmtPcs+AmtPacks) As 'TotalAmount' ," +
                                      "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag', Username, TransactionRemarks " +
                                      "From SalesOrdersList " +
                                      "Where OrderDate Between '" + fromDateString + "' And '" + toDateString + "' And " +
                                      "Username='" + userName + "' " +
                                      "Group by DocumentNo, OrderDate, CustomerName, TransactionRemarks, Username " +
                                      "Order By OrderDate, DocumentNo";

                    /*
                    cmd.CommandText = "select DISTINCT a.DocumentNo, Convert(varchar,a.TransactionDate,23) as TransactionDate, a.CustomerName, b.BeatName, a.ProfitCenteRname, " +
                                      "a.ItemName, a.QuantityInPieces, a.QuantityInPacks, a.TransactionRemarks, a.Username from Trade_SalesOrder_Table a, Trade_Customers_Table b Where " +
                                      "a.CustomerName=b.CustomerName and convert(varchar,a.TransactionDate,23) <= '" + asonDate.ToString() +
                                      "' Order By a.DocumentNo";
                    */
                    /*
                    cmd.CommandText = "select DocumentNo, Format(TransactionDate,'dd-MMM-yyyy') As 'OrderDate', " +
                                      "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As DownloadedFlag " +
                                      "From Trade_SalesOrder_Table " +
                                      "Where Convert(varchar,TransactionDate,23)>='" + fromDate + "' And " +
                                      "Convert(varchar,TransactionDate,23)<='" + toDate + "' And " +
                                      "Username='" + userName + "' " +
                                      "Group by DocumentNo, TransactionDate " +
                                      "Order By OrderDate, DocumentNo";
                    */

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
    }
}
