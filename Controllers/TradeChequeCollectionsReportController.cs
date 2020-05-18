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
    public class TradeChequeCollectionsReportController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string fromDate, string toDate, string userName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable ChequeCollections = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    //DateTime asonDate = DateTime.Parse(asAtDate);

                    cmd.CommandText = "select DocumentNo, Convert(varchar,TransactionDate,105) As 'CollectionDate', CustomerName, " +
                                      "Amount, ChequeNumber, Format(ChequeDate,'dd-MMM-yyyy') As 'ChequeDate', " +
                                      "RTRIM(ISNULL(AgainstInvoiceNumber,'')) As AgainstInvoiceNumber,  TransactionRemarks, " +
                                      "CASE WHEN DownloadedFlag > 0 THEN '1' ELSE '0' END As DownloadedFlag, Username " +
                                      "From ChequeCollections_Table " +
                                      "Where CollectionDate Between '" + fromDate + "' And '" + toDate + "' And " +
                                      "Username='" + userName + "' " +
                                      "Order By CollectionDate, DocumentNo";

                    da.SelectCommand = cmd;
                    ChequeCollections.TableName = "ChequeCollections";
                    da.Fill(ChequeCollections);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    ChequeCollections = ChequeCollections
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
