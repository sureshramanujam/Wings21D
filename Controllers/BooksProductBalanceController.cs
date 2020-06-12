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
    public class BooksProductBalanceController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable ProductBalance = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Books_ProductBalance_Table Order By ProductName", con);
                    da.SelectCommand = cmd;
                    ProductBalance.TableName = "ProductBalance";
                    da.Fill(ProductBalance);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    ProductBalance = ProductBalance
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
        public HttpResponseMessage Post(List<BooksProductBalance> productbalance)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            String uploadAll = String.Empty;

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            if (headers.Contains("uploadall"))
            {
                uploadAll = headers.GetValues("uploadall").First();
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (!String.IsNullOrEmpty(dbName))
            {
                if (uploadAll.ToLower().Trim() == "true")
                {
                    try
                    {   
                        con.Open();
                        cmd.CommandText = "Delete From Books_ProductBalance_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();

                        foreach (BooksProductBalance bpb in productbalance)
                        {
                            cmd.CommandText = "Insert Into Books_ProductBalance_Table Values('" + bpb.productName + "', '" +
                                              bpb.locationName + "'," + bpb.availableQty + ")";
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
                    try
                    {
                        con.Open();
                        foreach (BooksProductBalance bpb in productbalance)
                        {
                            cmd.CommandText = "Insert Into Books_ProductBalance_Table Values('" + bpb.productName + "', '" +
                                              bpb.locationName + "'," + bpb.availableQty + ")";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                    catch(SqlException ex)
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
