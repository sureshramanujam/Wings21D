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
    public class BooksCustomersSalesPendingOrdersController : ApiController
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

                    cmd.CommandText = "Select * from Books_CustomersPendingSalesOrder_Desktop_Table Where CustomerName='" + custName + "' " + 
                                      "Order by OrderDate, OrderNumber";
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
        public HttpResponseMessage Post(List<BooksCustomersPendingSalesOrders> CPSO)
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
                cmd.CommandText = "Select * From Books_CustomersPendingSalesOrder_Desktop_Table";
                da.SelectCommand = cmd;
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cmd.CommandText = "Delete From Books_CustomersPendingSalesOrder_Desktop_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    try
                    {
                        con.Open();

                        if (dt.Rows.Count == 0)
                        {
                            cmd.CommandText = "Create Table Books_CustomersPendingSalesOrder_Desktop_Table (" +
                                               "OrderNumber nchar(100) null," +
                                               "OrderDate date null," +
                                               "DueDate date null," +
                                               "CustomerName nvarchar(265) null," +
                                               "ProductName nvarchar(265) null," +
                                               "PendingQty decimal(18,2) null," +
                                               "LineAmount decimal(18,2) null," +
                                               "BranchName nvarchar(265) null," +
                                               "Username nvarchar(265) null)";
                        }
                        else
                        {
                            foreach (BooksCustomersPendingSalesOrders a in CPSO)
                            {
                                cmd.CommandText = "Insert Into Books_CustomersPendingSalesOrder_Desktop_Table Values('" + a.orderno + "','" +
                                                  String.Format("{0:yyyy-MM-dd}", a.date) + "','" + String.Format("{0:yyyy-MM-dd}", a.duedate) + "','" +
                                                  a.party + "','" + a.product + "'," + a.pendingqty + "," + a.lineamount + ",'" + a.branch + "','" + a.userName + "')";

                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                    return new HttpResponseMessage(HttpStatusCode.Created);
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
