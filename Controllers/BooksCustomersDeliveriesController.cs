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
    public class BooksCustomersDeliveriesController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string custName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Deliveries = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(custName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    cmd.CommandText = "Select * from Books_CustomersDeliveries_Desktop_Table Where CustomerName='" + custName + "' " + 
                                      "Order by OrderDate, OrderNumer";
                    da.SelectCommand = cmd;
                    Deliveries.TableName = "Deliveries";
                    da.Fill(Deliveries);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Deliveries = Deliveries
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
        public HttpResponseMessage Post(List<BooksCustomersDeliveries> BCD)
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
            
            if (!String.IsNullOrEmpty(dbName))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();

                cmd.CommandText = "Select name from sys.tables where name='Books_CustomersDeliveries_Desktop_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 0)
                {
                    con.Open();
                    cmd.CommandText = "Delete From Books_CustomersDeliveries_Desktop_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();

                    con.Open();

                    foreach(BooksCustomersDeliveries a in BCD)
                    {
                        cmd.CommandText = "Insert Into Books_CustomersDeliveries_Desktop_Table Values('" + a.orderno + "','" + a.dcno + "','" +
                                          String.Format("{0:yyyy-MM-dd}", a.date) + "','" + a.party + "','" +
                                          a.product + "'," + a.qty + "," + a.lineamount + ",'" + a.userName + "')";

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                else 
                {   
                        try
                        {
                            con.Open();
                           
                            cmd.CommandText = "Create Table Books_CustomersDeliveries_Desktop_Table (" +
                                              "OrderNumber nchar(100) null," +
                                              "DCNumber nchar(100) null," +
                                              "DCDate date null," +
                                              "CustomerName nvarchar(265) null," +
                                              "ProductName nvarchar(265) null," +
                                              "Quantity decimal(18,2) null," +
                                              "LineAmount decimal(18,2) null," +
                                              "Username nvarchar(265) null)";
                            cmd.ExecuteNonQuery();

                            foreach (BooksCustomersDeliveries a in BCD)
                            {
                                cmd.CommandText = "Insert Into Books_CustomersDeliveries_Desktop_Table Values('" + a.orderno + "','" + a.dcno + "','" +
                                                  String.Format("{0:yyyy-MM-dd}", a.date) + "','" + a.party + "','" +
                                                  a.product + "'," + a.qty + "," + a.lineamount + ",'" + a.userName + "')";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }
                        return new HttpResponseMessage(HttpStatusCode.Created);
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            return new HttpResponseMessage(HttpStatusCode.Created);
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
