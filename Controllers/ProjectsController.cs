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
    public class ProjectsController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Projects = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Projects_Table Order By ProjectName", con);
                    da.SelectCommand = cmd;
                    Projects.TableName = "Projects";
                    da.Fill(Projects);
                    con.Close();

                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Projects = Projects
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
        public HttpResponseMessage Post(List<Projects> prj)
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
                cmd.CommandText = "Select name from sys.tables where name='Projects_Table'";
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
                            cmd.CommandText = "Delete from Projects_Table";
                            cmd.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            foreach (Projects lcs in prj)
                            {
                                cmd.CommandText = "Insert Into Projects_Table Values('" + lcs.ProjectName + "')";
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
                            foreach (Projects lcs in prj)
                            {
                                cmd.CommandText = "Insert Into Projects_Table Values('" + lcs.ProjectName + "')";
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
                    cmd.CommandText = "Create Table Projects_Table (" +
                                         "ProjectName nvarchar(200) null)";
                    cmd.ExecuteNonQuery();
                    con.Close();

                    if (uploadAll.Trim().ToLower() == "true")
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandText = "Delete from Projects_Table";
                            cmd.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            foreach (Projects lcs in prj)
                            {
                                cmd.CommandText = "Insert Into Projects_Table Values('" + lcs.ProjectName + "')";
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
                            foreach (Projects lcs in prj)
                            {
                                cmd.CommandText = "Insert Into Projects_Table Values('" + lcs.ProjectName + "')";
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
