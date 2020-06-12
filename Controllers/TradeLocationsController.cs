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
    public class TradeLocationsController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Locations = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Trade_Locations_Table Order By LocationName", con);
                    da.SelectCommand = cmd;
                    Locations.TableName = "Locations";
                    da.Fill(Locations);
                    con.Close();

                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Locations = Locations
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
        public HttpResponseMessage Post(List<TradeLocations> locations)
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
                if (uploadAll.Trim().ToLower() == "true")
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "Delete from Trade_Locations_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        foreach (TradeLocations lcs in locations)
                        {
                            cmd.CommandText = "Insert Into Trade_Locations_Table Values('" + lcs.locationName + "')";
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
                        foreach (TradeLocations lcs in locations)
                        {
                            cmd.CommandText = "Insert Into Trade_Locations_Table Values('" + lcs.locationName + "')";
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
        }
    }
}
