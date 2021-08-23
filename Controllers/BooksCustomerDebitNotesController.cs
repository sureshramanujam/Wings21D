using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Wings21D.Models;

namespace Wings21D.Controllers
{
    public class BooksCustomerDebitNotesController : ApiController
    {
        // GET API <Controler>
        public HttpResponseMessage Get(string dbName, string custName, string fromDate, string toDate)
        {
            if (String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(custName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable DebitNotes = new DataTable();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                String fromDt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                String toDt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");

                cmd.CommandText = "Select CustomerName,InvoiceNumber, Convert(varchar,InvoiceDate,23) as InvoiceDate,InvoiceType," +
                     "AgainstInvoiceNumber,AgainstInvoiceDate,AgainstInvoiceValue,InvoiceValue,UserName from Books_CustomerDebitNotes_Table " +
                     "Where InvoiceDate between '" + fromDt + "' and '" + toDt + "' and  CustomerName='" + custName + "' " +
                     "Group by InvoiceNumber, CustomerName,InvoiceDate,InvoiceType,AgainstInvoiceNumber,AgainstInvoiceDate,AgainstInvoiceValue,InvoiceValue,UserName " +
                     "Order by InvoiceNumber,InvoiceDate";

                da.SelectCommand = cmd;
                DebitNotes.TableName = "DebitNotes";
                da.Fill(DebitNotes);
                con.Close();
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            var returnResponseObject = new
            {
                DebitNotes = DebitNotes
            };
            var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));

            return response;
        }

        //POST Api<Controller>

        public string Post(List<BooksCustomerDebitNotes> CDN)
        {

            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            String uploadAll = String.Empty;
            bool uploadAllData = false;

            if (headers.Contains("dbName"))
            {
                dbName = headers.GetValues("dbName").First();
            }
            if (headers.Contains("uploadall"))
            {
                uploadAll = headers.GetValues("uploadall").First();
                uploadAllData = uploadAll == "true" ? true : false;
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            DateTime todayDate = DateTime.Now;
            var dateOnly = todayDate.Date;

            if (!String.IsNullOrEmpty(dbName))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd.CommandText = "Select name from sys.tables where name='Books_CustomerDebitNotes_Table'";
                da.SelectCommand = cmd;
                da.Fill(dt);
                con.Close();

                string invoiceNumbers = String.Empty;
                if (dt.Rows.Count == 0)
                {
                    con.Open();
                    cmd.CommandText = "Create Table Books_CustomerDebitNotes_Table (" +
                                          "TransactionType nvarchar(60) null," +
                                          "CustomerName nvarchar(265) null," +
                                          "InvoiceNumber nvarchar(100) null," +
                                          "InvoiceDate date null," +
                                          "InvoiceType nchar(20) null," +
                                          "AgainstInvoiceNumber nvarchar(100) null," +
                                          "AgainstInvoiceDate date null," +
                                          "AgainstInvoiceValue decimal(18,2) null," +
                                          "AdjustmentAccountName nvarchar(265) null," +
                                          "ProductName nvarchar(265) null," +
                                          "InvoiceQuantity decimal(18,2) null," +
                                          "InvoiceValue decimal(18,2) null," +
                                          "BodyRowName nvarchar(100) null,"+
                                          "GSTRate nvarchar(100) null," +
                                          "DiscountAmount decimal(18,2) null," +
                                          "TaxableValue decimal(18,2) null," +
                                          "GSTAmount decimal(18,2) null," +
                                          "CessAmount decimal(18,2) null," +
                                          "LineAmount decimal(18,2) null," +
                                          "Remarks nvarchar(265) null," +
                                          "UserName nvarchar(265) null)";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                {
                    if (uploadAllData)
                    {
                        con.Open();
                        cmd.CommandText = "Delete From  Books_CustomerDebitNotes_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }


                con.Open();

                foreach (BooksCustomerDebitNotes a in CDN)
                {
                    cmd.CommandText = "Insert into Books_CustomerDebitNotes_Table Values ('" + a.TransactionType + "','" + a.CustomerName + "','" +
                                      a.InvoiceNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.InvoiceDate) + "','" + a.InvoiceType + "','" +
                                      a.AgainstInvoiceNumber + "','" + String .Format("{0:yyyy-MM-dd}", a.AgainstInvoiceDate) + "'," +
                                      a.AgainstInvoiceValue + ",'" + a.AdjustmentAccountName + "','" + a.ProductName + "'," + a.InvoiceQuantity + "," +
                                      a.InvoiceValue + ",'"+ a.BodyRowName +"','"+ a.GSTRate + "'," + a.DiscountAmount + "," + a.TaxableValue + "," + 
                                      a.GSTAmount + "," + a.CessAmount + "," + a.LineAmount + ",'" +
                                      a.Remarks + "','" + a.UserName + "')";

                    cmd.ExecuteNonQuery();
                    invoiceNumbers += a.InvoiceNumber + "$";
                    invoiceNumbers = invoiceNumbers.Replace('\\', ' ');

                }
                con.Close();
                return invoiceNumbers;
            }
            else
            {
                return ("Database not found.");

            }
        }
    }
}