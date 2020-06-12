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
    public class CompanyDetailsController : ApiController
    {          
        public HttpResponseMessage Post(List<CompanyDetails> CD)
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
                SqlDataAdapter companyDetailsAdapter = new SqlDataAdapter();
                DataTable companyDetailsDataTable = new DataTable();
                cmd.CommandText = "Select name from sys.tables where name='CompanyDetails_Table'";
                companyDetailsAdapter.SelectCommand = cmd;
                companyDetailsAdapter.Fill(companyDetailsDataTable);
                con.Close();

                if (uploadAll.ToLower().Trim()=="true")
                {
                    if (companyDetailsDataTable.Rows.Count > 0)
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandText = "Delete From CompanyDetails_Table";
                            cmd.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            foreach (CompanyDetails a in CD)
                            {
                                /*
                                a.CompanyLegalName = String.IsNullOrEmpty(a.CompanyLegalName) ? "NULL" : a.CompanyLegalName;
                                a.CompanyTradeName = String.IsNullOrEmpty(a.CompanyTradeName) ? "NULL" : a.CompanyTradeName;
                                a.CompanyAddressLine1 = String.IsNullOrEmpty(a.CompanyAddressLine1) ? "NULL" : a.CompanyAddressLine1;
                                a.CompanyAddressLine2 = String.IsNullOrEmpty(a.CompanyAddressLine2) ? "NULL" : a.CompanyAddressLine2;
                                a.CompanyAddressLine3 = String.IsNullOrEmpty(a.CompanyAddressLine3) ? "NULL" : a.CompanyAddressLine3;
                                a.CompanyCity = String.IsNullOrEmpty(a.CompanyCity) ? "NULL" : a.CompanyCity;
                                a.CompanyState = String.IsNullOrEmpty(a.CompanyState) ? "NULL" : a.CompanyState;
                                a.CompanyCountry = String.IsNullOrEmpty(a.CompanyCountry) ? "NULL" : a.CompanyCountry;
                                a.CompanyPINCode = String.IsNullOrEmpty(a.CompanyPINCode) ? "NULL" : a.CompanyPINCode;
                                */

                                cmd.CommandText = "Insert Into CompanyDetails_Table Values('" +
                                                  a.CompanyName + "','" + a.CompanyLegalName + "','" + a.CompanyTradeName + "','" +
                                                  a.CompanyAddressLine1 + "','" + a.CompanyAddressLine2 + "','" + a.CompanyAddressLine3 + "','" +
                                                  a.CompanyCity + "','" + a.CompanyState + "','" + a.CompanyCountry + "','" + a.CompanyPINCode + "','" +
                                                  String.Format("{0:yyyy-MM-dd}", a.CompanyFYStartDate) + "','" +
                                                  String.Format("{0:yyyy-MM-dd}", a.CompanyFYEndDate) + "')";

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
                    else
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandText = "Create Table CompanyDetails_Table (" +
                                                  "CompanyName nvarchar(265) null," +
                                                  "CompanyLegalName nvarchar(265) null," +
                                                  "CompanyTradeName nvarchar(265) null, " +
                                                  "CompanyAddressLine1 nvarchar(265) null, " +
                                                  "CompanyAddressLine2 nvarchar(265) null, " +
                                                  "CompanyAddressLine3 nvarchar(265) null, " +
                                                  "CompanyCity nvarchar(265) null, " +
                                                  "CompanyState nvarchar(265) null, " +
                                                  "CompanyCountry nvarchar(265) null, " +
                                                  "CompanyPINCode nvarchar(100) null, " +
                                                  "CompanyFYStartDate date null, " +
                                                  "CompanyFYEndDate date null)";
                            cmd.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            foreach (CompanyDetails a in CD)
                            {
                                /*
                                a.CompanyLegalName = String.IsNullOrEmpty(a.CompanyLegalName) ? "NULL" : a.CompanyLegalName;
                                a.CompanyTradeName = String.IsNullOrEmpty(a.CompanyTradeName) ? "NULL" : a.CompanyTradeName;
                                a.CompanyAddressLine1 = String.IsNullOrEmpty(a.CompanyAddressLine1) ? "NULL" : a.CompanyAddressLine1;
                                a.CompanyAddressLine2 = String.IsNullOrEmpty(a.CompanyAddressLine2) ? "NULL" : a.CompanyAddressLine2;
                                a.CompanyAddressLine3 = String.IsNullOrEmpty(a.CompanyAddressLine3) ? "NULL" : a.CompanyAddressLine3;
                                a.CompanyCity = String.IsNullOrEmpty(a.CompanyCity) ? "NULL" : a.CompanyCity;
                                a.CompanyState = String.IsNullOrEmpty(a.CompanyState) ? "NULL" : a.CompanyState;
                                a.CompanyCountry = String.IsNullOrEmpty(a.CompanyCountry) ? "NULL" : a.CompanyCountry;
                                a.CompanyPINCode = String.IsNullOrEmpty(a.CompanyPINCode) ? "NULL" : a.CompanyPINCode;
                                */

                                cmd.CommandText = "Insert Into CompanyDetails_Table Values('" +
                                                  a.CompanyName + "','" + a.CompanyLegalName + "','" + a.CompanyTradeName + "','" +
                                                  a.CompanyAddressLine1 + "','" + a.CompanyAddressLine2 + "','" + a.CompanyAddressLine3 + "','" +
                                                  a.CompanyCity + "','" + a.CompanyState + "','" + a.CompanyCountry + "','" + a.CompanyPINCode + "','" +
                                                  String.Format("{0:yyyy-MM-dd}", a.CompanyFYStartDate) + "','" +
                                                  String.Format("{0:yyyy-MM-dd}", a.CompanyFYEndDate) + "')";

                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                            return new HttpResponseMessage(HttpStatusCode.Created);
                        }
                        catch (Exception ex)
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }
                    }
                }
                else
                {
                    if (companyDetailsDataTable.Rows.Count == 0)
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandText = "Create Table CompanyDetails_Table (" +
                                                  "CompanyName nvarchar(265) null," +
                                                  "CompanyLegalName nvarchar(265) null," +
                                                  "CompanyTradeName nvarchar(265) null, " +
                                                  "CompanyAddressLine1 nvarchar(265) null, " +
                                                  "CompanyAddressLine2 nvarchar(265) null, " +
                                                  "CompanyAddressLine3 nvarchar(265) null, " +
                                                  "CompanyCity nvarchar(265) null, " +
                                                  "CompanyState nvarchar(265) null, " +
                                                  "CompanyCountry nvarchar(265) null, " +
                                                  "CompanyPINCode nvarchar(100) null, " +
                                                  "CompanyFYStartDate date null, " +
                                                  "CompanyFYEndDate date null)";
                            cmd.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            foreach (CompanyDetails a in CD)
                            {
                                /*
                                a.CompanyLegalName = String.IsNullOrEmpty(a.CompanyLegalName) ? "NULL" : a.CompanyLegalName;
                                a.CompanyTradeName = String.IsNullOrEmpty(a.CompanyTradeName) ? "NULL" : a.CompanyTradeName;
                                a.CompanyAddressLine1 = String.IsNullOrEmpty(a.CompanyAddressLine1) ? "NULL" : a.CompanyAddressLine1;
                                a.CompanyAddressLine2 = String.IsNullOrEmpty(a.CompanyAddressLine2) ? "NULL" : a.CompanyAddressLine2;
                                a.CompanyAddressLine3 = String.IsNullOrEmpty(a.CompanyAddressLine3) ? "NULL" : a.CompanyAddressLine3;
                                a.CompanyCity = String.IsNullOrEmpty(a.CompanyCity) ? "NULL" : a.CompanyCity;
                                a.CompanyState = String.IsNullOrEmpty(a.CompanyState) ? "NULL" : a.CompanyState;
                                a.CompanyCountry = String.IsNullOrEmpty(a.CompanyCountry) ? "NULL" : a.CompanyCountry;
                                a.CompanyPINCode = String.IsNullOrEmpty(a.CompanyPINCode) ? "NULL" : a.CompanyPINCode;
                                */

                                cmd.CommandText = "Insert Into CompanyDetails_Table Values('" +
                                                  a.CompanyName + "','" + a.CompanyLegalName + "','" + a.CompanyTradeName + "','" +
                                                  a.CompanyAddressLine1 + "','" + a.CompanyAddressLine2 + "','" + a.CompanyAddressLine3 + "','" +
                                                  a.CompanyCity + "','" + a.CompanyState + "','" + a.CompanyCountry + "','" + a.CompanyPINCode + "','" +
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
                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                    else
                    {
                        try
                        {
                            con.Open();
                            foreach (CompanyDetails a in CD)
                            {
                                /*
                                a.CompanyLegalName = String.IsNullOrEmpty(a.CompanyLegalName) ? "NULL" : a.CompanyLegalName;
                                a.CompanyTradeName = String.IsNullOrEmpty(a.CompanyTradeName) ? "NULL" : a.CompanyTradeName;
                                a.CompanyAddressLine1 = String.IsNullOrEmpty(a.CompanyAddressLine1) ? "NULL" : a.CompanyAddressLine1;
                                a.CompanyAddressLine2 = String.IsNullOrEmpty(a.CompanyAddressLine2) ? "NULL" : a.CompanyAddressLine2;
                                a.CompanyAddressLine3 = String.IsNullOrEmpty(a.CompanyAddressLine3) ? "NULL" : a.CompanyAddressLine3;
                                a.CompanyCity = String.IsNullOrEmpty(a.CompanyCity) ? "NULL" : a.CompanyCity;
                                a.CompanyState = String.IsNullOrEmpty(a.CompanyState) ? "NULL" : a.CompanyState;
                                a.CompanyCountry = String.IsNullOrEmpty(a.CompanyCountry) ? "NULL" : a.CompanyCountry;
                                a.CompanyPINCode = String.IsNullOrEmpty(a.CompanyPINCode) ? "NULL" : a.CompanyPINCode;
                                */

                                cmd.CommandText = "Insert Into CompanyDetails_Table Values('" +
                                                  a.CompanyName + "','" + a.CompanyLegalName + "','" + a.CompanyTradeName + "','" +
                                                  a.CompanyAddressLine1 + "','" + a.CompanyAddressLine2 + "','" + a.CompanyAddressLine3 + "','" +
                                                  a.CompanyCity + "','" + a.CompanyState + "','" + a.CompanyCountry + "','" + a.CompanyPINCode + "','" +
                                                  String.Format("{0:yyyy-MM-dd}", a.CompanyFYStartDate) + "','" +
                                                  String.Format("{0:yyyy-MM-dd}", a.CompanyFYEndDate) + "')";
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
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}

