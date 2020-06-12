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
    public class BranchesController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Branches = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Branches_Table Order By BranchName", con);
                    da.SelectCommand = cmd;
                    Branches.TableName = "Branches";
                    da.Fill(Branches);
                    con.Close();

                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Branches = Branches
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
        public HttpResponseMessage Post(List<Branches> Branches)
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
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd.CommandText = "Select name from sys.tables where name='Branches_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 0)
                {
                    if (uploadAll.Trim().ToLower() == "true")
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandText = "Delete from Branches_Table";
                            cmd.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            foreach (Branches lcs in Branches)
                            {
                                cmd.CommandText = "Insert Into Branches_Table Values('" + lcs.BranchName + "')";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        catch (Exception e)
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
                            foreach (Branches lcs in Branches)
                            {
                                cmd.CommandText = "Insert Into Branches_Table Values('" + lcs.BranchName + "')";
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
                    con.Open();
                    cmd.CommandText = "Create Table Branches_Table (" +
                                         "BranchName nvarchar(200) null)";
                    cmd.ExecuteNonQuery();
                    con.Close();

                    if (uploadAll.Trim().ToLower() == "true")
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandText = "Delete from Branches_Table";
                            cmd.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            foreach (Branches lcs in Branches)
                            {
                                cmd.CommandText = "Insert Into Branches_Table Values('" + lcs.BranchName + "')";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        catch (Exception e)
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
                            foreach (Branches lcs in Branches)
                            {
                                cmd.CommandText = "Insert Into Trade_Branches_Table Values('" + lcs.BranchName + "')";
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
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
