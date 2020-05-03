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
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
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

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();            
            cmd.Connection = con;

            /*
            //SqlDataAdapter da = new SqlDataAdapter();
            //DataTable dt = new DataTable();
            if (dt.Rows.Count > 0)
            {
                cmd.CommandText = "Delete from Trade_Items_Table Where ItemName='" + titems.itemName + "'";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Insert Into Trade_Items_Table Values(NEWID(),'" + titems.itemName + "', '" +
                titems.productName + "','" + titems.hsnsac + "','" + titems.profitCenterName + "'," + titems.rateperpiece +
                "," + titems.rateperpack + ",'" + titems.gstrate + "'," + titems.piecesperpack + "," + titems.itemmrp + titems.activeStatus + ")";
                cmd.ExecuteNonQuery();

                dt.Clear();

                cmd.CommandText = "Select count(*) from Trade_Items_Table Where ItemName = '" + titems.itemName + "'";
                da.Fill(dt);
            }
            */

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    foreach (BooksProducts tproducts in ti)
                    {
                        cmd.CommandText = "Insert Into Books_Products_Table Values(NEWID(), '" +
                        tproducts.productName + "','" + tproducts.hsnsac + "'," + tproducts.salesprice + ",'" + tproducts.gstrate + "'," + tproducts.productmrp + "," + tproducts.activeStatus + ")";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                catch
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
