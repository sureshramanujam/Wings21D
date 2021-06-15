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
using Wings21D.Utils;

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
                    //LogsGenerator.LogMessage("Begin", dbName);
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "Select 'All Categories' As CategoryName Union " +
                                       "Select Distinct(CategoryName) from Trade_ProductCategories_Table Order By CategoryName";
                    da.SelectCommand = cmd;
                    PCategories.TableName = "ProductCategories";
                    da.Fill(PCategories);
                    con.Close();
                    //LogsGenerator.LogMessage("End", dbName);
                }
                catch (Exception ex)
                {
                    //LogsGenerator.LogError(ex.ToString(), dbName);
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
                    //LogsGenerator.LogMessage("Begin", dbName);
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
                    //LogsGenerator.LogMessage("End", dbName);

                }
                catch(Exception ex)
                {
                    //LogsGenerator.LogError(ex.ToString(), dbName);
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                if (uploadEverything)
                {
                    try
                    {
                        //LogsGenerator.LogMessage("Begin", dbName);
                        con.Open();
                        cmd.CommandText = "Delete from Trade_ProductCategories_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        foreach (TradeProductCategories prc in productCategories)
                        {   
                          //  LogsGenerator.LogMessage(prc.ProductName + ":" + prc.CategoryName, dbName);
                            prc.ProductName = prc.ProductName.Replace("'", "''");
                            prc.CategoryName = prc.CategoryName.Replace("'", "''");
                            try
                            {
                                cmd.CommandText = "Insert Into Trade_ProductCategories_Table Values('" + prc.ProductName + "','" + prc.CategoryName + "')";
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException se)
                            {

                            }

                        }
                        con.Close();
                       // LogsGenerator.LogMessage("End", dbName);
                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                    catch (Exception e)
                    {
                       // LogsGenerator.LogError(e.ToString(), dbName);
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    try
                    {
                        //LogsGenerator.LogMessage("Begin", dbName);
                        con.Open();
                        foreach (TradeProductCategories prc in productCategories)
                        {
                            prc.ProductName = prc.ProductName.Replace("'", "''");
                            prc.CategoryName = prc.CategoryName.Replace("'", "''");
                            cmd.CommandText = "Insert Into Trade_Beats_Table Values('" + prc.ProductName + "','"+prc.CategoryName+"')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                        //LogsGenerator.LogMessage("End", dbName);
                    }
                    catch (Exception ex)
                    {
                        //LogsGenerator.LogError(ex.ToString(), dbName);
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                    return new HttpResponseMessage(HttpStatusCode.Created);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
