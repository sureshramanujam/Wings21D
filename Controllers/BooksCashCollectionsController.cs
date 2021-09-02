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
        public HttpResponseMessage Get(string dbName, string fromDate, string toDate, string userName)
        {
            if (String.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate) || string.IsNullOrEmpty(userName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid parameters.");
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable CashCollections = new DataTable();
            {
                string msg = "Username:" + userName;
                try
                {
                    string[] fdates = fromDate.Split('-');
                    string fromDt = fdates[2] + "-" + fdates[1] + "-" + fdates[0];

                    string[] tdates = toDate.Split('-');
                    string toDt = tdates[2] + "-" + tdates[1] + "-" + tdates[0];

                    //string fromDt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                    //msg += "::fromDate:" + fromDate + "::toDate:" + toDate;

                    //string toDt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");
                    //msg += "::fromDt:" + fromDt + "::toDt:" + toDt;

                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    cmd.CommandText = "With CashCollectionList As( " +
                                       "Select a.DocumentNo,  TransactionDate , " +
                                       "a.CustomerName, a.Amount, RTRIM(ISNULL(a.AgainstInvoiceNumber,'')) As AgainstInvoiceNumber," +
                                       "a.TransactionRemarks,a.DownloadedFlag, a.Username " +
                                       "From CashCollections_Table a " +
                                       "Left Join Books_Customers_Table b on a.CustomerName=b.CustomerName " +
                                       ") " +
                                     "Select DocumentNo,Convert(varchar,TransactionDate,105) TransactionDate, CustomerName," +
                                     " Amount,AgainstInvoiceNumber,TransactionRemarks,Username," +
                                     "CASE WHEN Sum(DownloadedFlag) > 0 THEN '1' ELSE '0' END As 'DownloadedFlag' " +
                                     "From CashCollectionList " +
                                     "Where TransactionDate Between '" + fromDt + "' And '" + toDt + "' And " +
                                     "Username='" + userName + "' " +
                                     "Group by DocumentNo, TransactionDate, CustomerName, Amount, AgainstInvoiceNumber, TransactionRemarks, Username " +
                                     "Order By TransactionDate, DocumentNo";

                    msg += cmd.CommandText;

                    da.SelectCommand = cmd;

                    CashCollections.TableName = "CashCollections";
                    da.Fill(CashCollections);
                    con.Close();
                    if (CashCollections == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.OK, "Collections is null");
                    }
                    if (CashCollections.Rows.Count == 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.OK, "rowcount is 0");
                    }
                }
                catch (Exception ex)
                {
                    msg += Environment.NewLine + "Exception:" + ex.ToString();
                    msg += Environment.NewLine + "ExceptionMessage:" + ex.Message;

                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg);
                }

                var returnResponseObject = new
                {
                    CashCollections = CashCollections
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
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
