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
using System.Web.Script.Serialization;

namespace Wings21D.Controllers
{
    public class BooksCustomersController : ApiController
    {

        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {   
            if (!String.IsNullOrEmpty(dbName))
            {
                SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
                DataSet ds = new DataSet();
                List<string> mn = new List<string>();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable Customers = new DataTable();
                SqlCommand cmd = new SqlCommand();

                try
                {
                    cmd.Connection = con;
                    con.Open();
                    cmd.CommandText = "Select DISTINCT a.CustomerName, ISNULL(a.CustomerCity,'Not Set') CustomerCity, " +
                                      "ISNULL(a.GSTNumber,'Not Set') GSTNumber, ISNULL(Sum(b.PendingValue),0) TotalDue " +
                                      "From Books_Customers_Table a LEFT Join Books_CustomersPendingBills_Table b " +
                                      "On a.CustomerName = b.CustomerName "  +
                                      "Group by a. CustomerName, b.CustomerName, a.GSTNumber, a.CustomerCity " +
                                      "Order by a.CustomerName";

                    da.SelectCommand = cmd;
                    Customers.TableName = "Customers";
                    da.Fill(Customers);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Customers = Customers
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));

                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        // GET api/values/5

        // POST api/values
        //public HttpResponseMessage Post([FromBody]string jsonData)
        public HttpResponseMessage Post(List<BooksCustomers> customers)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;

            //byte[] byteArray = Convert.FromBase64String(jsonData);
            //string customersInfo = System.Text.Encoding.UTF8.GetString(byteArray);

            //List<BooksCustomers> customers = new JavaScriptSerializer().Deserialize<List<BooksCustomers>>(customersInfo);

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            if (!String.IsNullOrEmpty(dbName))
            {
                SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                try
                {
                    con.Open();
                    foreach (BooksCustomers cust in customers)
                    {
                        cmd.CommandText = "Insert Into Books_Customers_Table Values('" + cust.customerName + "','" + cust.gstNumber + "','" + cust.customerCity +
                        "'," + cust.activeStatus + ")";
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
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
