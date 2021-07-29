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
    public class BooksCashCollectionsController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string fromDate,string toDate,string userName)
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
                    string fromDt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                    string toDt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");
                    
                    cmd.CommandText = "select a.DocumentNo, Convert(varchar,a.TransactionDate,23) as TransactionDate, a.CustomerName, " +
                                      "a.Amount, RTRIM(ISNULL(a.AgainstInvoiceNumber,'')) As AgainstInvoiceNumber, " +
                                      "a.TransactionRemarks, a.Username From CashCollections_Table a, Books_Customers_Table b Where " +
                                      "a.CustomerName=b.CustomerName and a.username='"+userName+"' and a.TransactionDate between '" + fromDt +"' and '"+toDt+"' "+
                                      " And a.DownloadedFlag=0 Order By a.DocumentNo";

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

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        /*
        [HttpPost]
        public string suresh([FromBody]string val)
        {
            return val;
        }
        */

        // POST api/<controller>                
        [HttpPost]
        public HttpResponseMessage SaveCollection(CashCollectionsEntry myCE)
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

                    cmd.CommandText = "Insert Into CashCollections_Table Values(" +
                                      //"(Select ISNULL(Max(TransactionNo),0)+1 From CashCollections_Table Where Year(convert(varchar,TransactionDate,23))='" + String.Format("{0:yyyy}",todayDate.Date) + "')," +
                                      "(Select ISNULL(Max(TransactionNo),0)+1 From CashCollections_Table), " +
                                      "'" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "',  null, '" + myCE.customerName + "', "
                                      + Convert.ToDouble(myCE.collectionAmount) + ", '" +
                                      myCE.transactionRemarks + "','" + myCE.userName + "','CR-M-',0, " +
                                      //"'CR-M-' +  CAST((Select ISNULL(Max(TransactionNo),0)+1 From CashCollections_Table Where YEAR(convert(varchar,TransactionDate,23))='" + String.Format("{0:yyyy}", todayDate.Date) + "') AS varchar), '" + 
                                      "'CR-M-' +  CAST((Select ISNULL(Max(TransactionNo),0)+1 From CashCollections_Table) AS varchar), '" +
                                      myCE.againstInvoiceNumber + "')";

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
