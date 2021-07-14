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
    public class BooksSalesInvoicesReportController : ApiController
    {
        // GET: BooksSalesInvoicesReport
        public HttpResponseMessage Get(string dbName, string fromDate, string toDate, string userName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesInvoice = new DataTable();
            string fromdt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
            string todt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");

            string fromdt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
            string todt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    //DateTime asonDate = DateTime.Parse(asAtDate);

                    cmd.CommandText = "With SalesOrdersList As( " +
                                       "Select a.DocumentNo,TransactionDate As 'InvoiceDate', " +
                                       "a.CustomerName, a.ProductName, b.SalesPrice, a.Quantity," +
                                       "(a.Quantity*b.SalesPrice) As 'Amount', " +
                                       "c.CashAmount, c.ChequeAmount, a.InvoiceType, a.CashCreditType ," +
                                       "TransactionRemarks, DownloadedFlag, a.Username " +
                                       "From Books_SalesInvoice_Table a " +
                                       "Left Join Books_Products_Table b on a.ProductName=b.ProductName " +
                                       "Left Join Books_SalesInvoice_Payments_Table c on a.DocumentNo=c.DocumentNo " +
                                     ") " +
                                     "Select DocumentNo,Convert(varchar,InvoiceDate,105) As OrderDate, CustomerName, Sum(Quantity) As 'TotalQty', " +
                                     "Sum(Amount) As 'TotalAmount' ," +
                                     "CashAmount, ChequeAmount, InvoiceType, CashCreditType," +
                                     "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag', Username, TransactionRemarks " +
                                     "From SalesOrdersList " +
                                     "Where InvoiceDate Between '" + fromdt + "' And '" + todt + "' And " +
                                     "Username='" + userName + "' " +
                                     "Group by DocumentNo, InvoiceDate, CustomerName, TransactionRemarks, Username, CashAmount, ChequeAmount,InvoiceType,CashCreditType " +
                                     "Order By InvoiceDate, DocumentNo";
                    /*
                    cmd.CommandText = "select DISTINCT a.DocumentNo, Convert(varchar,a.TransactionDate,23) as TransactionDate, a.CustomerName, b.BeatName, a.ProfitCenteRname, " +
                                      "a.ItemName, a.QuantityInPieces, a.QuantityInPacks, a.TransactionRemarks, a.Username from Trade_SalesOrder_Table a, Trade_Customers_Table b Where " +
                                      "a.CustomerName=b.CustomerName and convert(varchar,a.TransactionDate,23) <= '" + asonDate.ToString() +
                                      "' Order By a.DocumentNo";
                    


                    cmd.CommandText = "select DocumentNo, Format(TransactionDate,'dd-MMM-yyyy') As 'OrderDate', " +
                                      "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As DownloadedFlag " +
                                      "From Books_SalesOrder_Table " +
                                      //"Where Convert(varchar,a.TransactionDate,23) <= '" + asonDate.ToString() + "' " +
                                      "Where Username='" + userName + "' " +
                                      "Group by DocumentNo, TransactionDate " +
                                      "Order By OrderDate Desc, DocumentNo";
                    */

                    da.SelectCommand = cmd;
                    SalesInvoice.TableName = "SalesInvoice";
                    da.Fill(SalesInvoice);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    SalesInvoice = SalesInvoice
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