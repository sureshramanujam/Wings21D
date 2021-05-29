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
    public class TradeProductCategoriesController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable PCategories = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Select Distinct(CategoryName) from Trade_ProductCategories_Table Order By CategoryName", con);
                    da.SelectCommand = cmd;
                    PCategories.TableName = "ProductCategories";
                    da.Fill(PCategories);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    ProductCategories = PCategories
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
        public HttpResponseMessage Post(List<TradeProductCategories> productCategories)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            String uploadAll = String.Empty;
            bool uploadEverything = false;

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            if (headers.Contains("uploadall"))
            {
                uploadAll = headers.GetValues("uploadall").First();
                uploadEverything = uploadAll == "true" ? true : false;
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    con.Open();
                    cmd.CommandText = "Select name from sys.tables where name='Trade_ProductCategories_Table'";
                    sqlDataAdapter.SelectCommand = cmd;
                    sqlDataAdapter.Fill(dataTable);
                    con.Close();

                    if(dataTable.Rows.Count==0)
                    {
                        con.Open();
                        cmd.CommandText = "Create Table Trade_ProductCategories_Table(ProductName varchar(100),CategoryName varchar(100))";
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                }
                catch(Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                if (uploadEverything)
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "Delete from Trade_ProductCategories_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        foreach (TradeProductCategories prc in productCategories)
                        {
                            cmd.CommandText = "Insert Into Trade_ProductCategories_Table Values('" + prc.ProductName + "','"+prc.CategoryName+"')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                        return new HttpResponseMessage(HttpStatusCode.Created);
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
                        foreach (TradeProductCategories prc in productCategories)
                        {
                            cmd.CommandText = "Insert Into Trade_Beats_Table Values('" + prc.ProductName + "','"+prc.CategoryName+"')";
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
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
