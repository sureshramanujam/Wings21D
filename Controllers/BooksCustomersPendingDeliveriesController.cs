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
    public class BooksCustomersPendingDeliveriesController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string custName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesOrders = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(custName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    cmd.CommandText = "Select * from Books_CustomersPendingDeliveries_Desktop_Table Where CustomerName='" + custName + "' " + 
                                      "Order by DCNumber";
                    da.SelectCommand = cmd;
                    SalesOrders.TableName = "SalesOrders";
                    da.Fill(SalesOrders);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    SalesOrders = SalesOrders
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
        public string Post(List<BooksCustomersPendingDeliveries> BCPD)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            string deliveryNumbers = String.Empty;

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (!String.IsNullOrEmpty(dbName))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd.CommandText = "Select name from sys.tables where name='Books_CustomersPendingDeliveries_Desktop_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 0)
                {
                    con.Open();
                    cmd.CommandText = "Delete From Books_CustomersPendingDeliveries_Desktop_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Open();
                    foreach (BooksCustomersPendingDeliveries a in BCPD)
                    {
                        cmd.CommandText = "Insert Into Books_CustomersPendingDeliveries_Desktop_Table Values('" + a.CustomerName + "','" + a.ProductName + "','" +
                                          a.BatchName + "','" + a.DCNumber + "','" + a.InvoiceNumber + "'," + a.BilledQuantity + ",'" + a.BranchName + "'," + a.LineAmount +
                                          ",'" + a.Username + "')";

                        cmd.ExecuteNonQuery();
                        deliveryNumbers += a.DCNumber + "$";
                        deliveryNumbers = deliveryNumbers.Replace('\\', ' ');
                    }
                    con.Close();
                }
                else
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "Create Table Books_CustomersPendingDeliveries_Desktop_Table (" +
                                               "CustomerName nvarchar(265) null," +
                                               "ProductName nvarchar(265) null," +
                                               "Batch nvarchar(265) null," +
                                               "DCNumber nchar(100) null," +
                                               "InvoiceNumber nchar(100) null," +
                                               "BilledQty decimal(18,2) null," +
                                               "BranchName nvarchar(265) null," +
                                               "LineAmount decimal(18,2) null," +
                                               "Username nvarchar(265) null)";
                        cmd.ExecuteNonQuery();

                        foreach (BooksCustomersPendingDeliveries a in BCPD)
                        {
                            cmd.CommandText = "Insert Into Books_CustomersPendingDeliveries_Desktop_Table Values('" + a.CustomerName + "','" + a.ProductName + "','" +
                                          a.BatchName + "','" + a.DCNumber + "','" + a.InvoiceNumber + "'," + a.BilledQuantity + ",'" + a.BranchName + "'," + a.LineAmount +
                                          ",'" + a.Username + "')";

                            cmd.ExecuteNonQuery();
                            deliveryNumbers += a.DCNumber + "$";
                            deliveryNumbers = deliveryNumbers.Replace('\\', ' ');
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        return "Unable to insert data.";
                    }
                    return deliveryNumbers;
                }
                return deliveryNumbers;
            }
            else
            {
                return "Database error.";
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
