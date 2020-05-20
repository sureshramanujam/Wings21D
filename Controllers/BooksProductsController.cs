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
    public class BooksProductsController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Products = new DataTable();            

            if (!String.IsNullOrEmpty(dbName))
            {   
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    con.Open();                    
                    cmd.Connection = con;
                    cmd.CommandText = "Select a.ProductName, a.HSNSAC, a.GSTRate," +
                                      "Sum(a.SalesPrice)SalesPrice, Sum(a.ProductMRP) ProductMRP, " +
                                      "ISNULL(Sum(b.AvailableQty),0) BalanceQty " +
                                      "From Books_Products_Table a " +
                                      "Left Join Books_ProductBalance_Table b On a.ProductName=b.ProductName " +
                                      "Group by a.ProductName, a.HSNSAC, a.GSTRate, b.ProductName " +
                                      "Order by a.ProductName";

                    da.SelectCommand = cmd;
                    Products.TableName = "Products";
                    da.Fill(Products);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Products = Products
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        // POST api/values
        public HttpResponseMessage Post(List<BooksProducts> ti)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            String uploadAll = String.Empty;

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            if(headers.Contains("uploadall"))
            {
                uploadAll = headers.GetValues("uploadall").First();
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();            
            cmd.Connection = con;
            
            if (!String.IsNullOrEmpty(dbName))
            {
                if(!String.IsNullOrEmpty(uploadAll))
                {
                    if (uploadAll.Trim().ToLower() == "true")
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandText = "Delete from Books_Products_Table";
                            cmd.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            foreach (BooksProducts tproducts in ti)
                            {
                                tproducts.productName = tproducts.productName.Replace("'", "''");
                                cmd.CommandText = "Insert Into Books_Products_Table Values(NEWID(), '" +
                                tproducts.productName + "','" + tproducts.hsnsac + "'," + tproducts.rateperpiece + ",'" + tproducts.gstrate + "'," + tproducts.productmrp + "," + tproducts.activeStatus + ")";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();

                        }
                        catch (Exception e)
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                    {
                        try
                        {
                            con.Open();
                            foreach (BooksProducts tproducts in ti)
                            {
                                tproducts.productName = tproducts.productName.Replace("'", "''");
                                cmd.CommandText = "Insert Into Books_Products_Table Values(NEWID(), '" +
                                tproducts.productName + "','" + tproducts.hsnsac + "'," + tproducts.rateperpiece + ",'" + tproducts.gstrate + "'," + tproducts.productmrp + "," + tproducts.activeStatus + ")";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        catch
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }
                    }
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
