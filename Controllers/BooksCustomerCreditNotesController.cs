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
    public class BooksCustomerCreditNotesController : ApiController
    {
        // GET API<Controllers>
        public HttpResponseMessage Get(string dbName,string custName,string fromDate,string toDate)
        {
            if(String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(custName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable CreditNotes = new DataTable();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                string fromDt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                string toDt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");

                cmd.CommandText = "Select CustomerName,InvoiceNumber, Convert(varchar,InvoiceDate,23) as InvoiceDate,InvoiceType," +
                      "AgainstInvoiceNumber,AgainstInvoiceDate,AgainstInvoiceValue,InvoiceValue,UserName from Books_CustomerCreditNotes_Table " +
                      "Where InvoiceDate between '" + fromDt + "' and '" + toDt + "' and  CustomerName='" + custName + "' " +
                      "Group by InvoiceNumber, CustomerName,InvoiceDate,InvoiceType,AgainstInvoiceNumber,AgainstInvoiceDate,AgainstInvoiceValue,InvoiceValue,UserName " +
                      "Order by InvoiceNumber,InvoiceDate";

                da.SelectCommand = cmd;
                CreditNotes.TableName = "CreditNotes";
                da.Fill(CreditNotes);
                con.Close();
            }
            catch (Exception Ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            var returnresponseObject = new
            {
                CreditNotes = CreditNotes
            };
            var response = Request.CreateResponse(HttpStatusCode.OK, returnresponseObject, MediaTypeHeaderValue.Parse("application/json"));
            return response;
        }

        public string Post(List<BooksCustomerCreditNotes> CCN)
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
                cmd.CommandText = "Select name from sys.tables where name='Books_CustomerCreditNotes_Table'";
                da.SelectCommand = cmd;
                da.Fill(dt);
                con.Close();

                string invoiceNumbers = String.Empty;
                if (dt.Rows.Count == 0)
                {
                    con.Open();
                    cmd.CommandText = "Create Table Books_CustomerCreditNotes_Table (" +
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
                                          "GSTRate nvarchar(100) null," +
                                          "DiscountAmount decimal(18,2) null," +
                                          "TaxableValue decimal(18,2) null," +
                                          "GSTAmount decimal(18,2) null," +
                                          "CessAmount decimal(18,2) null," +
                                          "LineAmount decimal(18,2) null," +
                                          "Remarks nvarchar(265) null," +
                                          "UserName nvarchar(265) null" +
                                          "InvoiceValue decimal(18, 2) null," +
                                          "BodyRowName nvarchar(100) null, )";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                {
                    if (uploadAllData)
                    {
                        con.Open();
                        cmd.CommandText = "Delete From  Books_CustomerCreditNotes_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }


                con.Open();

                foreach (BooksCustomerCreditNotes a in CCN)
                {
                    cmd.CommandText = "Insert into Books_CustomerCreditNotes_Table Values ('" + a.TransactionType + "','" + a.CustomerName + "','" +
                                      a.InvoiceNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.InvoiceDate) + "','" + a.InvoiceType + "','" +
                                      a.AgainstInvoiceNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.AgainstInvoiceDate) + "'," +
                                      a.AgainstInvoiceValue + ",'" + a.AdjustmentAccountName + "','" + a.ProductName + "'," + a.InvoiceQuantity + ",'" +
                                      a.GSTRate + "'," + a.DiscountAmount + "," + a.TaxableValue + "," +
                                      a.GSTAmount + "," + a.CessAmount + "," + a.LineAmount + ",'" +
                                      a.Remarks + "','" + a.UserName + "',"+ a.InvoiceValue + ",'" + a.BodyRowName + "')";

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