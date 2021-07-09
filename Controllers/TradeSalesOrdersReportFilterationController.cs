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
    public class TradeSalesOrdersReportFilterationController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string fromDate, string toDate, string userName,string profitCenter,string salesExecutive)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesOrders = new DataTable();

            string fromDateString = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
            string toDateString = DateTime.Parse(toDate).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(salesExecutive))
            {
                salesExecutive = salesExecutive.Remove(salesExecutive.Length - 1, 1);
            }
            if (!string.IsNullOrEmpty(profitCenter))
            {
                profitCenter = profitCenter.Remove(profitCenter.Length - 1, 1);
            }
            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    //DateTime asonDate = DateTime.Parse(asAtDate);
                    if (userName.ToUpper() == "SUPERVISOR")
                    {
                        if (profitCenter.ToUpper().Contains("ALL PROFIT CENTERS"))
                        {
                            if (string.IsNullOrEmpty(salesExecutive) || salesExecutive.ToUpper().Contains("SUPERVISOR"))
                            {
                                cmd.CommandText = "With SalesOrdersList As( " +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces*b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks*b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username,a.ProfitCenterName " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName=b.ItemName " +
                                      ") " +
                                      "Select DocumentNo, Convert(varchar,OrderDate,105) As OrderDate, CustomerName, Sum(QuantityInPieces+QuantityInPacks) As 'TotalQty', " +
                                      "Sum(AmtPcs+AmtPacks) As 'TotalAmount' ," +
                                      "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag', Username, TransactionRemarks " +
                                      "From SalesOrdersList " +
                                      "Where OrderDate Between '" + fromDateString + "' And '" + toDateString + "'" +
                                      "Group by DocumentNo, OrderDate, CustomerName, TransactionRemarks, Username " +
                                      "Order By OrderDate, DocumentNo";
                            }
                            else
                            {
                                cmd.CommandText = "With SalesOrdersList As( " +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces*b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks*b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username,a.ProfitCenterName " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName=b.ItemName " +
                                      ") " +
                                      "Select DocumentNo, Convert(varchar,OrderDate,105) As OrderDate, CustomerName, Sum(QuantityInPieces+QuantityInPacks) As 'TotalQty', " +
                                      "Sum(AmtPcs+AmtPacks) As 'TotalAmount' ," +
                                      "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag', Username, TransactionRemarks " +
                                      "From SalesOrdersList " +
                                      "Where OrderDate Between '" + fromDateString + "' And '" + toDateString + "' And " +
                                      "Username in (" + salesExecutive + ")" +
                                      "Group by DocumentNo, OrderDate, CustomerName, TransactionRemarks, Username " +
                                      "Order By OrderDate, DocumentNo";
                            }
                            

                        }
                        else
                        {
                            if (string.IsNullOrEmpty(salesExecutive) || salesExecutive.ToUpper().Contains("SUPERVISOR"))
                            {
                                cmd.CommandText = "With SalesOrdersList As( " +
                                       "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                       "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                       "(a.QuantityInPieces*b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks*b.RatePerPack) As 'AmtPacks', " +
                                       "TransactionRemarks, DownloadedFlag, a.Username,a.ProfitCenterName " +
                                       "From Trade_SalesOrder_Table a " +
                                       "Left Join Trade_Items_Table b on a.ItemName=b.ItemName " +
                                     ") " +
                                     "Select DocumentNo, Convert(varchar,OrderDate,105) As OrderDate, CustomerName, Sum(QuantityInPieces+QuantityInPacks) As 'TotalQty', " +
                                     "Sum(AmtPcs+AmtPacks) As 'TotalAmount' ," +
                                     "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag', Username, TransactionRemarks " +
                                     "From SalesOrdersList " +
                                     "Where OrderDate Between '" + fromDateString + "' And '" + toDateString + "' And " +
                                      "ProfitCenterName in (" + profitCenter + ")" +
                                     "Group by DocumentNo, OrderDate, CustomerName, TransactionRemarks, Username " +
                                     "Order By OrderDate, DocumentNo";
                            }
                            else {
                                cmd.CommandText = "With SalesOrdersList As( " +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces*b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks*b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username,a.ProfitCenterName " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName=b.ItemName " +
                                      ") " +
                                      "Select DocumentNo, Convert(varchar,OrderDate,105) As OrderDate, CustomerName, Sum(QuantityInPieces+QuantityInPacks) As 'TotalQty', " +
                                      "Sum(AmtPcs+AmtPacks) As 'TotalAmount' ," +
                                      "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag', Username, TransactionRemarks " +
                                      "From SalesOrdersList " +
                                      "Where OrderDate Between '" + fromDateString + "' And '" + toDateString + "' And " +
                                      "Username in (" + salesExecutive + ") And " + "ProfitCenterName in (" + profitCenter + ")" +
                                      "Group by DocumentNo, OrderDate, CustomerName, TransactionRemarks, Username " +
                                      "Order By OrderDate, DocumentNo";
                            }
                            

                        }
                        

                    }
                   
                    else
                    {
                        if (profitCenter.ToUpper().Contains("ALL PROFIT CENTERS"))
                        {
                            cmd.CommandText = "With SalesOrdersList As( " +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces*b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks*b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username,a.ProfitCenterName " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName=b.ItemName " +
                                      ") " +
                                      "Select DocumentNo, Convert(varchar,OrderDate,105) As OrderDate, CustomerName, Sum(QuantityInPieces+QuantityInPacks) As 'TotalQty', " +
                                      "Sum(AmtPcs+AmtPacks) As 'TotalAmount' ," +
                                      "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag', Username, TransactionRemarks " +
                                      "From SalesOrdersList " +
                                      "Where OrderDate Between '" + fromDateString + "' And '" + toDateString + "' And " +
                                      "Username='" + userName + "'"+
                                      "Group by DocumentNo, OrderDate, CustomerName, TransactionRemarks, Username " +
                                      "Order By OrderDate, DocumentNo";
                        }
                        else
                        {
                            cmd.CommandText = "With SalesOrdersList As( " +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces*b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks*b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username,a.ProfitCenterName " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName=b.ItemName " +
                                      ") " +
                                      "Select DocumentNo, Convert(varchar,OrderDate,105) As OrderDate, CustomerName, Sum(QuantityInPieces+QuantityInPacks) As 'TotalQty', " +
                                      "Sum(AmtPcs+AmtPacks) As 'TotalAmount' ," +
                                      "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag', Username, TransactionRemarks " +
                                      "From SalesOrdersList " +
                                      "Where OrderDate Between '" + fromDateString + "' And '" + toDateString + "' And " +
                                      "Username='" + userName + "' And " + "ProfitCenterName in (" + profitCenter + ")" +
                                      "Group by DocumentNo, OrderDate, CustomerName, TransactionRemarks, Username " +
                                      "Order By OrderDate, DocumentNo";
                        }
                        

                    }
                    

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
