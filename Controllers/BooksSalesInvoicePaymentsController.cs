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
    public class BooksSalesInvoicePaymentsController : ApiController
    {
        // POST api/<controller>                
        [HttpPost]
        public HttpResponseMessage SaveInvoicePayments(List<BooksSalesInvoiceEntryPayments> SIL)
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
                    cmd.CommandText = "Select name from sys.tables where name='Books_SalesInvoice_Payments_Table'";
                    da.SelectCommand = cmd;
                    dt.Clear();
                    da.Fill(dt);
                    con.Close();

                    con.Open();

                    foreach (BooksSalesInvoiceEntryPayments si in SIL)
                    {
                        cmd.CommandText = "Select DocumentNo From Books_SalesInvoice_Table Where UserName='" +
                                          si.Username + "' Order by TransactionNo DESC";
                    }

                    SqlDataAdapter docNumberAdapter = new SqlDataAdapter();
                    DataTable newDocumentNumber = new DataTable();
                    docNumberAdapter.SelectCommand = cmd;
                    docNumberAdapter.Fill(newDocumentNumber);
                    con.Close();

                    if (dt.Rows.Count > 0)
                    {
                        con.Open();
                        foreach (BooksSalesInvoiceEntryPayments si in SIL)
                        {
                            cmd.CommandText = "Insert Into Books_SalesInvoice_Payments_Table Values ('" +
                                              newDocumentNumber.Rows[0][0].ToString() + "'," + si.CashAmount + "," + si.ChequeAmount + ",'" + si.ChequeNumber +
                                              "','" + String.Format("{0:yyyy-MM-dd}", si.ChequeDate) + "')";

                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                    else
                    {
                        con.Open();
                        cmd.CommandText = "Create Table Books_SalesInvoice_Payments_Table (" +
                                             "DocumentNo nvarchar(200) null," +
                                             "CashAmount Decimal(18,2) null," +
                                             "ChequeAmount Decimal(18,2) null," +
                                             "ChequeNumber nvarchar(100) null," +
                                             "ChequeDate date null)";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        foreach (BooksSalesInvoiceEntryPayments si in SIL)
                        {
                            cmd.CommandText = "Insert Into Books_SalesInvoice_Payments_Table Values ('" +
                                              newDocumentNumber.Rows[0][0].ToString() + "'," + si.CashAmount + "," + si.ChequeAmount + ",'" + si.ChequeNumber +
                                              "','" + String.Format("{0:yyyy-MM-dd}", si.ChequeDate) + "')";

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
