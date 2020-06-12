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
    public class BooksCustomerSalesInvoicesDesktopController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string custName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable DesktopSalesInvoices = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(custName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    cmd.CommandText = "Select * from Books_CustomersSalesInvoices_Desktop_Table Where CustomerName='" + custName + "' " + 
                                      "Order by OrderDate, OrderNumber";
                    da.SelectCommand = cmd;
                    DesktopSalesInvoices.TableName = "DesktopSalesOrders";
                    da.Fill(DesktopSalesInvoices);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    DesktopSalesInvoices = DesktopSalesInvoices
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

        // POST api/<controller>                
        public string Post(List<BooksCustomersSalesInvoicesDesktop> CSID)
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
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd.CommandText = "Select name from sys.tables where name='Books_CustomersSalesInvoices_Desktop_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                string invoiceNumbers = String.Empty;

                if (dt.Rows.Count > 0)
                {
                    con.Open();
                    cmd.CommandText = "Delete From Books_CustomersSalesInvoices_Desktop_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Open();
                    
                    foreach (BooksCustomersSalesInvoicesDesktop a in CSID)
                    {
                        cmd.CommandText = "Insert Into Books_CustomersSalesInvoices_Desktop_Table Values('" + a.TransactionType + "','" + a.CustomerName + "','" +
                                          a.InvoiceNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.InvoiceDate) + "','" + a.InvoiceType + "','" +
                                          a.InvoiceSubType + "','" + a.EWayBillNumber + "','" + a.ProductName + "'," + a.InvoiceQuantity + ",'" +
                                          a.GSTRate + "'," + a.GSTAmount + "," + a.CessAmount + "," + a.LineAmount + ",'" +
                                          a.Remarks + "','" + a.OrderNumber + "','" + a.DeliveryNumber + "','" + a.Username + "')";

                        cmd.ExecuteNonQuery();
                        invoiceNumbers += a.InvoiceNumber + "$";
                        invoiceNumbers = invoiceNumbers.Replace('\\', ' ');
                    }
                    con.Close();
                }
                else
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "Create Table Books_CustomersSalesInvoices_Desktop_Table (" +
                                              "TransactionType nvarchar(60) null," +
                                              "CustomerName nvarchar(265) null," +
                                              "InvoiceNumber nchar(100) null," +
                                              "InvoiceDate date null," +
                                              "InvoiceType nchar(20) null," +
                                              "InvoiceSubType nchar(20) null," +
                                              "EWayBillNumber nvarchar(30) null," +
                                              "ProductName nvarchar(265) null," +
                                              "InvoiceQty decimal(18,2) null," +
                                              "GSTRate nvarchar(30) null," +
                                              "DiscountAmount decimal(18,2) null," +
                                              "GSTAmount decimal(18,2) null," +
                                              "CessAmount decimal(18,2) null," +
                                              "LineAmount decimal(18,2) null," +                                              
                                              "Remarks nvarchar(265) null," +
                                              "OrderNumber nchar(50) null," +
                                              "DeliveryNumber nchar(50) null," +
                                              "Username nvarchar(265) null)";
                        cmd.ExecuteNonQuery();

                        foreach (BooksCustomersSalesInvoicesDesktop a in CSID)
                        {
                            try
                            {
                                cmd.CommandText = "Insert Into Books_CustomersSalesInvoices_Desktop_Table Values('" + a.TransactionType + "','" + a.CustomerName + "','" +
                                          a.InvoiceNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.InvoiceDate) + "','" + a.InvoiceType + "','" +
                                          a.InvoiceSubType + "','" + a.EWayBillNumber + "','" + a.ProductName + "'," + a.InvoiceQuantity + ",'" +
                                          a.GSTRate + "'," + a.GSTAmount + "," + a.CessAmount + "," + a.LineAmount + ",'" +
                                          a.Remarks + "','" + a.OrderNumber + "','" + a.DeliveryNumber + "','" + a.Username + "')";

                                cmd.ExecuteNonQuery();
                                invoiceNumbers += a.InvoiceNumber + "$";
                                invoiceNumbers = invoiceNumbers.Replace('\\', ' ');
                            }
                            catch
                            {

                            }
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {  
                        return "Unable to insert data.";
                    }
                    return "Error";
                }
                return invoiceNumbers;
            }
            else
            {
                return "Database not found.";
            }
        }
    }
}
