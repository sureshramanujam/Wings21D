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
    public class BooksChequeCollectionsController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string fromDate,string toDate,string userName)
        {
            if (String.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate) || string.IsNullOrEmpty(userName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid parameters.");
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable ChequeCollections = new DataTable();

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                   // string fromDt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                   // string toDt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");
                string[] fdates = fromDate.Split('-');
                string fromDt = fdates[2] + "-" + fdates[1] + "-" + fdates[0];

                string[] tdates = toDate.Split('-');
                string toDt = tdates[2] + "-" + tdates[1] + "-" + tdates[0];

                cmd.CommandText = "select DISTINCT a.DocumentNo, Convert(varchar,a.TransactionDate,23) as TransactionDate, a.CustomerName, " +
                                      "a.Amount, RTRIM(ISNULL(a.ChequeNumber,'')) As ChequeNumber, Convert(varchar,a.ChequeDate,23)  As ChequeDate, RTRIM(ISNULL(a.AgainstInvoiceNumber,'')) As AgainstInvoiceNumber, " +
                                      "a.TransactionRemarks, a.Username from ChequeCollections_Table a, Books_Customers_Table b Where " +
                                      "a.CustomerName=b.CustomerName and a.username='"+userName+"' and a.TransactionDate between '" + fromDt +"' and '"+toDt+"'"+
                                      " And a.DownloadedFlag=0 Order By a.DocumentNo";

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

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>                
        [HttpPost]
        public HttpResponseMessage SaveCollection(ChequeCollectionsEntry myCE)
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

                    cmd.CommandText = "Insert Into ChequeCollections_Table Values(" +
                                      //"(Select ISNULL(Max(TransactionNo),0)+1 From ChequeCollections_Table Where year(convert(varchar,TransactionDate,23))='" + String.Format("{0:yyyy}", todayDate.Date) + "')," +
                                      "(Select ISNULL(Max(TransactionNo),0)+1 From ChequeCollections_Table), " +
                                      "'" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "', null, '" + myCE.customerName + "', "
                                      + Convert.ToDouble(myCE.collectionAmount) +
                                      ", '" + myCE.chequeNumber + "', " +
                                      "'" + String.Format("{0:yyyy-MM-dd}",myCE.chequeDate) + "','" + myCE.againstInvoiceNumber + "','" + myCE.transactionRemarks + "','" + myCE.userName + "','BR-M-',0, " +
                                      //"'BR-M-' +  CAST((Select ISNULL(Max(TransactionNo),0)+1 From ChequeCollections_Table Where YEAR(convert(varchar,TransactionDate,23))='" + String.Format("{0:yyyy}", todayDate.Date) + "') AS varchar)" + ")";
                                      "'BR-M-' +  CAST((Select ISNULL(Max(TransactionNo),0)+1 From ChequeCollections_Table) AS varchar)" + ")";

                    cmd.ExecuteNonQuery();
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
