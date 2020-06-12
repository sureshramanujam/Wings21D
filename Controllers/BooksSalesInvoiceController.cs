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
    public class BooksSalesInvoiceController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string asAtDate)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesInvoices = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    DateTime asonDate = DateTime.Parse(asAtDate);

                    cmd.CommandText = "Select a.DocumentNo, Convert(varchar,a.TransactionDate,23) as TransactionDate, a.InvoiceType, " +
                                      "a.CustomerName, a.ProductName, ISNULL(a.Quantity, 0) As Quantity, ISNULL(a.TransactionRemarks, '') As TransactionRemarks, " +
                                      "ISNULL(b.CashAmount,0) as CashAmount, ISNULL(b.ChequeAmount,0) As ChequeAmount, ISNULL(b.ChequeNumber,'') As ChequeNumber, " +
                                      "ISNULL(Convert(varchar,b.ChequeDate,23),'') As ChequeDate, a.Username, " +
                                      "ISNULL(a.BranchName,'') As BranchName, ISNULL(a.LocationName,'') As LocationName, " +
                                      "ISNULL(a.DivisionName,'') As DivisionName, ISNULL(a.ProjectName,'') As ProjectName " +
                                      "From Books_SalesInvoice_Table a Left Join Books_SalesInvoice_Payments_Table b on b.DocumentNo = a.DocumentNo " +
                                      "Where convert(varchar,a.TransactionDate,23) <= '" + asonDate.ToString() + "' And a.DownloadedFlag=0 Order By a.DocumentNo";
                    da.SelectCommand = cmd;
                    SalesInvoices.TableName = "SalesInvoices";
                    da.Fill(SalesInvoices);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    SalesInvoices = SalesInvoices
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
                
            }
        }
       
        // POST api/<controller>                
        [HttpPost]
        public HttpResponseMessage SaveInvoice(List<BooksSalesInvoiceEntry> SIL)
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
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();
                    cmd.CommandText = "Select name from sys.tables where name='Books_SalesInvoice_Table'";
                    da.SelectCommand = cmd;
                    dt.Clear();
                    da.Fill(dt);
                    con.Close();

                    string transRemarks = String.Empty;
                    string transactionSeries = String.Empty;
                    string transcationNumber = String.Empty;

                    if (dt.Rows.Count > 0)
                    {
                        con.Open();
                        //cmd.CommandText = "Select ISNULL(Max(TransactionNo), 0) + 1 From Trade_SalesOrder_Table Where YEAR(convert(varchar,TransactionDate,23)) = '" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "'";
                        cmd.CommandText = "Select ISNULL(Max(TransactionNo), 0) + 1 From Books_SalesInvoice_Table";
                        SqlDataAdapter docNumberAdapter = new SqlDataAdapter();
                        DataTable newDocumentNumber = new DataTable();
                        docNumberAdapter.SelectCommand = cmd;
                        docNumberAdapter.Fill(newDocumentNumber);
                        con.Close();

                        con.Open();
                        foreach (BooksSalesInvoiceEntry si in SIL)
                        {
                            transRemarks = si.TransactionRemarks.Replace("\\n", "");
                            si.CustomerName = si.CustomerName.Replace("'", "''");
                            si.ProductName = si.ProductName.Replace("'", "''");

                            transactionSeries = si.InvoiceType == "B2B" ? "SIT-M-" : "SIR-M-";
                            transcationNumber = transactionSeries + Convert.ToInt32(newDocumentNumber.Rows[0][0]).ToString();

                            cmd.CommandText = "Insert Into Books_SalesInvoice_Table Values(" + Convert.ToInt32(newDocumentNumber.Rows[0][0]) +
                                              ",'" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "','" + si.InvoiceType +  "','" + si.CashCreditType + "','" +
                                              si.CustomerName + "','" + si.ProductName + "'," +
                                              si.Quantity + ",'" + transRemarks + "','','','','','" + transactionSeries + "','" +
                                              transcationNumber + "',0,'" + si.UserName + "')";

                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                    else
                    {
                        con.Open();
                        cmd.CommandText = "Create Table Books_SalesInvoice_Table (" +
                                              "TransactionNo nchar(100) null," +
                                              "TransactionDate date null," +
                                              "InvoiceType nchar(10) null," +
                                              "CashCreditType nchar(10) null," +
                                              "CustomerName nvarchar(265) null," +
                                              "ProductName nvarchar(265) null," +
                                              "Quantity decimal(18,2) null," +
                                              "TransactionRemarks nvarchar(265) null," +
                                              "BranchName nvarchar(265) null," +
                                              "LocationName nvarchar(265) null," +
                                              "DivisionName nvarchar(265) null," +
                                              "ProjectName nvarchar(265) null," +
                                              "TransactionSeries nchar(10) null," +
                                              "DocumentNo nvarchar(200) null," +
                                              "DownloadedFlag int null," +
                                              "Username nvarchar(265) null)";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        cmd.CommandText = "Select ISNULL(Max(TransactionNo), 0) + 1 From Books_SalesInvoice_Table";
                        SqlDataAdapter docNumberAdapter = new SqlDataAdapter();
                        DataTable newDocumentNumber = new DataTable();
                        docNumberAdapter.SelectCommand = cmd;
                        docNumberAdapter.Fill(newDocumentNumber);
                        con.Close();

                        con.Open();
                        foreach (BooksSalesInvoiceEntry si in SIL)
                        {
                            transRemarks = si.TransactionRemarks.Replace("\\n", "");
                            si.CustomerName = si.CustomerName.Replace("'", "''");
                            si.ProductName = si.ProductName.Replace("'", "''");

                            transactionSeries = si.InvoiceType == "B2B" ? "SIT-M-" : "SIR-M-";
                            transcationNumber = transactionSeries + Convert.ToInt32(newDocumentNumber.Rows[0][0]).ToString();

                            cmd.CommandText = "Insert Into Books_SalesInvoice_Table Values(" + Convert.ToInt32(newDocumentNumber.Rows[0][0]) +
                                              ",'" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "','" + si.InvoiceType + "','" + si.CashCreditType + "','" +
                                              si.CustomerName + "','" + si.ProductName + "'," +
                                              si.Quantity + ",'" + transRemarks + "','','','','','" + transactionSeries + "','" +
                                              transcationNumber + "',0,'" + si.UserName + "')";

                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
