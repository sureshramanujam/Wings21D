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
    public class TradeCashCollectionsReportController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string fromDate, string toDate, string userName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable CashCollections = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    DateTime startDate = DateTime.Parse(fromDate);
                    DateTime endDate = DateTime.Parse(toDate);


                    cmd.CommandText = "Select DocumentNo, Format(TransactionDate,'dd-MMM-yyyy') As 'CollectionDate', CustomerName" +
                                      "Amount, RTRIM(ISNULL(AgainstInvoiceNumber,'')) As AgainstInvoiceNumber, TransactionRemarks, " +
                                      "CASE WHEN DownloadedFlag > 0 THEN '1' ELSE '0' END As DownloadedFlag, Username " +
                                      "From CashCollections_Table " + "Where Convert(varchar,TransactionDate,23) >= '" + String.Format("{0:yyyy-MM-dd}", startDate) + "' And " +
                                      "Convert(varchar,TransactionDate,23) <= '" + String.Format("{0:yyyy-MM-dd}", endDate) + "' And " + " Username='" + userName + "' " +
                                      "Order By CollectionDate, DocumentNo";

                    da.SelectCommand = cmd;
                    CashCollections.TableName = "CashCollections";
                    da.Fill(CashCollections);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    CashCollections = CashCollections
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
