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
using System.Web.Script.Serialization;

namespace Wings21D.Controllers
{
    public class BooksCustomersController : ApiController
    {

        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {   
            if (!String.IsNullOrEmpty(dbName))
            {
                SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
                DataSet ds = new DataSet();
                List<string> mn = new List<string>();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable Customers = new DataTable();
                SqlCommand cmd = new SqlCommand();

                try
                {
                    cmd.Connection = con;
                    con.Open();
                    cmd.CommandText = "Select DISTINCT a.CustomerName, ISNULL(a.CustomerCity,'Not Set') CustomerCity, " +
                                      "ISNULL(a.GSTNumber,'Not Set') GSTNumber, ISNULL(Sum(b.PendingValue),0) TotalDue " +
                                      "From Books_Customers_Table a LEFT Join Books_CustomersPendingBills_Table b " +
                                      "On a.CustomerName = b.CustomerName "  +
                                      "Group by a. CustomerName, b.CustomerName, a.GSTNumber, a.CustomerCity " +
                                      "Order by a.CustomerName";

                    da.SelectCommand = cmd;
                    Customers.TableName = "Customers";
                    da.Fill(Customers);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Customers = Customers
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
        //public HttpResponseMessage Post([FromBody]string jsonData)
        public HttpResponseMessage Post(List<BooksCustomers> customers)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            String uploadAll = String.Empty;

            //byte[] byteArray = Convert.FromBase64String(jsonData);
            //string customersInfo = System.Text.Encoding.UTF8.GetString(byteArray);

            //List<BooksCustomers> customers = new JavaScriptSerializer().Deserialize<List<BooksCustomers>>(customersInfo);

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }

            if (headers.Contains("uploadall"))
            {
                uploadAll = headers.GetValues("uploadall").First();
            }

            if (!String.IsNullOrEmpty(dbName))
            {
                SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd.CommandText = "Select name from sys.tables where name='Books_CustomerOfficeAddress_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count == 0)
                {
                    con.Open();
                    cmd.CommandText = "Create Table Books_CustomerOfficeAddress_Table (" +
                                           "CustomerName nvarchar(265) null," +
                                           "AddressLine1 nvarchar(265) null," +
                                           "AddressLine2 nvarchar(265) null," +
                                           "AddressLine3 nvarchar(265) null," +
                                           "City nvarchar(265) null," +
                                           "State nvarchar(265) null," +
                                           "Country nvarchar(265) null," +
                                           "PINCode nvarchar(265) null)";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Open();

                    foreach (BooksCustomers cust in customers)
                    {
                        cmd.CommandText = "Insert Into Books_CustomerOfficeAddress_Table Values('" + cust.CustomerName + "','" +
                                           cust.OfficeAddressLine1 + "','" + cust.OfficeAddressLine2 + "','" +
                                           cust.OfficeAddressLine3 + "','" + cust.OfficeCity + "','" +
                                           cust.OfficeState + "','" + cust.OfficeCountry + "','" +
                                           cust.OfficePINCode + "')";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }

                cmd.CommandText = "Select name from sys.tables where name='Books_CustomerShippingAddress_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count == 0)
                {
                    con.Open();
                    cmd.CommandText = "Create Table Books_CustomerShippingAddress_Table (" +
                                           "CustomerName nvarchar(265) null," +
                                           "AddressLine1 nvarchar(265) null," +
                                           "AddressLine2 nvarchar(265) null," +
                                           "AddressLine3 nvarchar(265) null," +
                                           "City nvarchar(265) null," +
                                           "State nvarchar(265) null," +
                                           "Country nvarchar(265) null," +
                                           "PINCode nvarchar(265) null)";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Open();

                    foreach (BooksCustomers cust in customers)
                    {
                        cmd.CommandText = "Insert Into Books_CustomerShippingAddress_Table Values('" + cust.CustomerName + "','" +
                                                       cust.ShippingAddressLine1 + "','" + cust.ShippingAddressLine2 + "','" +
                                                       cust.ShippingAddressLine3 + "','" + cust.ShippingCity + "','" +
                                                       cust.ShippingState + "','" + cust.ShippingCountry + "','" +
                                                       cust.ShippingPINCode + "')";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }

                cmd.CommandText = "Select name from sys.tables where name='Books_CustomerContactDetails_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count == 0)
                {
                    con.Open();
                    cmd.CommandText = "Create Table Books_CustomerContactDetails_Table (" +
                                           "CustomerName nvarchar(265) null," +
                                           "PrimaryContactNumber nchar(20) null," +
                                           "AlterContactNumber nchar(20) null," +
                                           "EmaiId nvarchar(265) null)";
                    cmd.ExecuteNonQuery();
                    con.Close();

                    con.Open();
                    foreach (BooksCustomers cust in customers)
                    {
                        cmd.CommandText = "Insert Into Books_CustomerContactDetails_Table Values('" + cust.CustomerName + "','" +
                                                       cust.PrimaryContactNumber + "','" + cust.AlternateContactnumber + "','" +
                                                       cust.EmailId + "',')";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }

                if (uploadAll.Trim().ToLower() == "true")
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "Delete From Books_Customers_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        foreach (BooksCustomers cust in customers)
                        {
                            cmd.CommandText = "Insert Into Books_Customers_Table Values('" + cust.CustomerName + "','" + cust.GSTNumber + "','" + cust.City +
                                              "'," + cust.ActiveStatus + ")";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                        con.Open();
                        cmd.CommandText = "Delete From Books_CustomerOfficeAddress_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        foreach (BooksCustomers cust in customers)
                        {
                            cmd.CommandText = "Insert Into Books_CustomerOfficeAddress_Table Values('" + cust.CustomerName + "','" +
                                              cust.OfficeAddressLine1 + "','" + cust.OfficeAddressLine2 + "','" +
                                              cust.OfficeAddressLine3 + "','" + cust.OfficeCity + "','" +
                                              cust.OfficeState + "','" + cust.OfficeCountry + "','" +
                                              cust.OfficePINCode + "')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                        con.Open();
                        cmd.CommandText = "Delete From Books_CustomerShippingAddress_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        foreach (BooksCustomers cust in customers)
                        {
                            cmd.CommandText = "Insert Into Books_CustomerShippingAddress_Table Values('" + cust.CustomerName + "','" +
                                               cust.ShippingAddressLine1 + "','" + cust.ShippingAddressLine2 + "','" +
                                               cust.ShippingAddressLine3 + "','" + cust.ShippingCity + "','" +
                                               cust.ShippingState + "','" + cust.ShippingCountry + "','" +
                                               cust.ShippingPINCode + "')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                        con.Open();
                        cmd.CommandText = "Delete From Books_CustomerContactDetails_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();
                        foreach (BooksCustomers cust in customers)
                        {
                            cmd.CommandText = "Insert Into Books_CustomerContactDetails_Table Values('" + cust.CustomerName + "','" +
                                               cust.PrimaryContactNumber + "','" + cust.AlternateContactnumber + "','" +
                                               cust.EmailId + "',')";
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
                        foreach (BooksCustomers cust in customers)
                        {
                            cmd.CommandText = "Insert Into Books_Customers_Table Values('" + cust.CustomerName + "','" + cust.GSTNumber + "','" + cust.City +
                            "'," + cust.ActiveStatus + ")";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                        con.Open();
                        foreach (BooksCustomers cust in customers)
                        {
                            cmd.CommandText = "Insert Into Books_CustomerOfficeAddress_Table Values('" + cust.CustomerName + "','" +
                                              cust.OfficeAddressLine1 + "','" + cust.OfficeAddressLine2 + "','" +
                                              cust.OfficeAddressLine3 + "','" + cust.OfficeCity + "','" +
                                              cust.OfficeState + "','" + cust.OfficeCountry + "','" +
                                              cust.OfficePINCode + "')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                        con.Open();
                        foreach (BooksCustomers cust in customers)
                        {
                            cmd.CommandText = "Insert Into Books_CustomerShippingAddress_Table Values('" + cust.CustomerName + "','" +
                                               cust.ShippingAddressLine1 + "','" + cust.ShippingAddressLine2 + "','" +
                                               cust.ShippingAddressLine3 + "','" + cust.ShippingCity + "','" +
                                               cust.ShippingState + "','" + cust.ShippingCountry + "','" +
                                               cust.ShippingPINCode + "')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                        con.Open();
                        foreach (BooksCustomers cust in customers)
                        {
                            cmd.CommandText = "Insert Into Books_CustomerContactDetails_Table Values('" + cust.CustomerName + "','" +
                                               cust.PrimaryContactNumber + "','" + cust.AlternateContactnumber + "','" +
                                               cust.EmailId + "',')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                    }
                    catch (Exception ex)
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
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
