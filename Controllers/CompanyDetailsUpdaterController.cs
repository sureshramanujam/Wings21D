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
    public class CompanyDetailsUpdaterController : ApiController
    {          
        public HttpResponseMessage Post(List<CompanyDetails> CD)
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
                cmd.CommandText = "Select name from sys.tables where name='CompanyDetails_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                string orderNumbers = String.Empty;
                

                if (dt.Rows.Count > 0)
                {
                    con.Open();
                    cmd.CommandText = "Delete From CompanyDetails_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Open();
                    
                    foreach (CompanyDetails a in CD)
                    {
                        cmd.CommandText = "Insert Into CompanyDetails_Table Values('" + dbName + "','" +
                                          a.CompanyName + "','" + a.CompanyLegaName + "','" + a.CompanyTradeName +"','" +
                                          a.CompanyAddressLine1 + "','" + a.CompanyAddressLine2 + "'," + a.CompanyAddressLine3 + "," + 
                                          a.CompanyCity + ",'" + a.CompanyState + "','" + a.CompanyCountry + "','" + a.CompanyPINCode + "','" +
                                          String.Format("{0:yyyy-MM-dd}", a.CompanyFYStartDate) + "','" +
                                          String.Format("{0:yyyy-MM-dd}", a.CompanyFYEndDate) +"')";

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                else
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "Create Table CompanyDetails_Table (" +
                                              "CompanyDatabaseName nvarchar(100) null," +
                                              "CompanyName nvarchar(265) null," +
                                              "CompanyLegalName nvarchar(265) null," +
                                              "CompanyTradeName nvarchar(265) null, " +
                                              "CompanyAddressLine1 nvarchar(265) null, " +
                                              "CompanyAddressLine2 nvarchar(265) null, " +
                                              "CompanyAddressLine3 nvarchar(265) null, " +
                                              "CompanyCity nvarchar(265) null, " +
                                              "CompanyState nvarchar(265) null, " +
                                              "CompanyCountry nvarchar(265 null, " +
                                              "CompanyPINCode nvarchar(100) null, " +
                                              "CompanyFYStartDate date null, " +
                                              "CompanyFYEndDate date null)";
                        cmd.ExecuteNonQuery();

                        foreach (CompanyDetails a in CD)
                        {
                            cmd.CommandText = "Insert Into CompanyDetails_Table Values('" + dbName + "','" +
                                              a.CompanyName + "','" + a.CompanyLegaName + "','" + a.CompanyTradeName + "','" +
                                              a.CompanyAddressLine1 + "','" + a.CompanyAddressLine2 + "'," + a.CompanyAddressLine3 + "," +
                                              a.CompanyCity + ",'" + a.CompanyState + "','" + a.CompanyCountry + "','" + a.CompanyPINCode + "','" +
                                              String.Format("{0:yyyy-MM-dd}", a.CompanyFYStartDate) + "','" +
                                              String.Format("{0:yyyy-MM-dd}", a.CompanyFYEndDate) + "')";

                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);                        
                    }
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
