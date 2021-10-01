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
        public HttpResponseMessage Get(string dbName)
        {
            if (String.IsNullOrEmpty(dbName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
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
                                  "On a.CustomerName = b.CustomerName " +
                                  "Group by a. CustomerName, b.CustomerName, a.GSTNumber, a.CustomerCity " +
                                  "Order by a.CustomerName";

                da.SelectCommand = cmd;
                Customers.TableName = "Customers";
                da.Fill(Customers);
                con.Close();
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            var returnResponseObject = new
            {
                Customers = Customers
            };

            var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
            return response;
        }

        // GET api/values/5

        // POST api/values
        //public HttpResponseMessage Post([FromBody]string jsonData)
        public HttpResponseMessage Post(List<BooksCustomers> customers)
        {
            var re = Request;
            var headers = re.Headers;
            //byte[] byteArray = Convert.FromBase64String(jsonData);
            //string customersInfo = System.Text.Encoding.UTF8.GetString(byteArray);
            //List<BooksCustomers> customers = new JavaScriptSerializer().Deserialize<List<BooksCustomers>>(customersInfo);

            string dbName = string.Empty;
            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }
            if (string.IsNullOrEmpty(dbName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            bool uploadAllData = false;
            if (headers.Contains("uploadall"))
            {
                string uploadAll = headers.GetValues("uploadall").First();
                uploadAllData = uploadAll.Trim().ToLower() == "true";
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            try
            {
                UpdateCustomers(cmd, customers, uploadAllData);
                UpdateOfficeAddress(cmd, customers, uploadAllData);
                UpdateShippingAddress(cmd, customers, uploadAllData);
                UpdateContactDetails(cmd, customers, uploadAllData);

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        void UpdateCustomers(SqlCommand cmd, List<BooksCustomers> customers, bool uploadAllData)
        {
            //Books_Customers_Table table created from db restore process

            if (uploadAllData)
            {
                cmd.Connection.Open();
                cmd.CommandText = "Delete From Books_Customers_Table";
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            {
                cmd.Connection.Open();
                foreach (BooksCustomers cust in customers)
                {
                    cust.CustomerName = cust.CustomerName.Replace("'", "''");
                    //cmd.CommandText = "Insert Into Books_Customers_Table Values('" + cust.CustomerName + "','" + cust.GSTNumber + "','" + cust.City +
                    cmd.CommandText = "Insert Into Books_Customers_Table Values('" + cust.CustomerName + "','" + cust.GSTNumber + "',''," + cust.ActiveStatus + ")";
                    cmd.ExecuteNonQuery();
                }
                cmd.Connection.Close();
            }
        }
        void UpdateOfficeAddress(SqlCommand cmd, List<BooksCustomers> customers, bool uploadAllData)
        {
            //Office Address
            SqlCommand officeAddressCommand = new SqlCommand();
            DataTable officeAddressDataTable = new DataTable();
            {
                officeAddressCommand.CommandText = "Select name from sys.tables where name='Books_CustomerOfficeAddress_Table'";
                officeAddressCommand.Connection = cmd.Connection;

                cmd.Connection.Open();
                SqlDataAdapter officeAddressAdapter = new SqlDataAdapter();
                officeAddressAdapter.SelectCommand = cmd;
                officeAddressAdapter.Fill(officeAddressDataTable);
                cmd.Connection.Close();
            }
            if (officeAddressDataTable.Rows.Count == 0) //table not existing then create table
            {
                cmd.Connection.Open();
                officeAddressCommand.CommandText = "Create Table Books_CustomerOfficeAddress_Table (" +
                                                   "CustomerName nvarchar(265) null," +
                                                   "AddressLine1 nvarchar(265) null,AddressLine2 nvarchar(265) null," +
                                                   "AddressLine3 nvarchar(265) null,City nvarchar(265) null," +
                                                   "State nvarchar(265) null,Country nvarchar(265) null," +
                                                   "PINCode nvarchar(265) null,ContactPerson nvarchar(265) null)";
                officeAddressCommand.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            if (uploadAllData) // table exists and remove all rows
            {
                cmd.Connection.Open();
                officeAddressCommand.CommandText = "Delete From Books_CustomerOfficeAddress_Table";
                officeAddressCommand.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            {
                cmd.Connection.Open();
                foreach (BooksCustomers cust in customers)
                {
                    officeAddressCommand.CommandText = "Insert Into Books_CustomerOfficeAddress_Table Values('" +
                                        cust.CustomerName + "','" +
                                       cust.BillingAddressLineOne + "','" + cust.BillingAddressLineTwo + "','" +
                                       cust.BillingAddressLineThree + "','" + cust.BillingAddressCity + "','" +
                                       cust.BillingAddressState + "','" + cust.BillingAddressCountry + "','" +
                                       cust.BillingAddressPincode + "','" + cust.BillingAddressContactPerson + "')";
                    officeAddressCommand.ExecuteNonQuery();
                }
                cmd.Connection.Close();
            }
        }
        void UpdateShippingAddress(SqlCommand cmd, List<BooksCustomers> customers, bool uploadAllData)
        {
            DataTable shippingAddressDataTable = new DataTable();
            SqlCommand shippingAddressCommand = new SqlCommand();
            {
                shippingAddressCommand.CommandText = "Select name from sys.tables where name='Books_CustomerShippingAddress_Table'";
                shippingAddressCommand.Connection = cmd.Connection;
                cmd.Connection.Open();
                SqlDataAdapter shippingAddressAdapter = new SqlDataAdapter();
                shippingAddressAdapter.SelectCommand = shippingAddressCommand;
                shippingAddressAdapter.Fill(shippingAddressDataTable);
                cmd.Connection.Close();
            }
            if (shippingAddressDataTable.Rows.Count == 0) // table not exists then create table
            {
                cmd.Connection.Open();
                shippingAddressCommand.CommandText = "Create Table Books_CustomerShippingAddress_Table (" +
                                                   "CustomerName nvarchar(265) null," +
                                                   "AddressLine1 nvarchar(265) null,AddressLine2 nvarchar(265) null," +
                                                   "AddressLine3 nvarchar(265) null,City nvarchar(265) null," +
                                                   "State nvarchar(265) null,Country nvarchar(265) null," +
                                                   "PINCode nvarchar(265) null,ContactPerson nvarchar(265) null)";
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            if (uploadAllData) // table exists and remove all rows
            {
                cmd.Connection.Open();
                shippingAddressCommand.CommandText = "Delete From Books_CustomerShippingAddress_Table";
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            {
                cmd.Connection.Open();
                foreach (BooksCustomers cust in customers)
                {
                    shippingAddressCommand.CommandText = "Insert Into Books_CustomerShippingAddress_Table Values('" +
                                                        cust.CustomerName + "','" +
                                                       cust.ShippingAddressLineOne + "','" + cust.ShippingAddressLineTwo + "','" +
                                                       cust.ShippingAddressLineThree + "','" + cust.ShippingAddressCity + "','" +
                                                       cust.ShippingAddressState + "','" + cust.ShippingAddressCountry + "','" +
                                                       cust.ShippingPincode + "','" + cust.ShippingAddressContactPerson + "')";
                    shippingAddressCommand.ExecuteNonQuery();
                }
                cmd.Connection.Close();
            }
        }
        void UpdateContactDetails(SqlCommand cmd, List<BooksCustomers> customers, bool uploadAllData)
        {
            DataTable contactDetailsDataTable = new DataTable();
            SqlCommand contactDetailsCommand = new SqlCommand();
            {
                contactDetailsCommand.CommandText = "Select name from sys.tables where name='Books_CustomerContactDetails_Table'";
                contactDetailsCommand.Connection = cmd.Connection;
                cmd.Connection.Open();
                SqlDataAdapter contactDetailsAdapter = new SqlDataAdapter();
                contactDetailsAdapter.SelectCommand = cmd;
                contactDetailsAdapter.Fill(contactDetailsDataTable);
                cmd.Connection.Close();
            }
            if (contactDetailsDataTable.Rows.Count == 0) // table not exists then create table
            {
                cmd.Connection.Open();
                contactDetailsCommand.CommandText = "Create Table Books_CustomerContactDetails_Table (" +
                                                   "CustomerName nvarchar(265) null," +
                                                   "PrimaryContactNumber nchar(20) null," +
                                                   "AlterContactNumber nchar(20) null," +
                                                   "EmaiId nvarchar(265) null)";
                contactDetailsCommand.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            if (uploadAllData) // table exists and remove all rows
            {
                cmd.Connection.Open();
                contactDetailsCommand.CommandText = "Delete From Books_CustomerContactDetails_Table";
                contactDetailsCommand.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            {
                cmd.Connection.Open();
                foreach (BooksCustomers cust in customers)
                {
                    contactDetailsCommand.CommandText = "Insert Into Books_CustomerContactDetails_Table Values('" +
                                                    cust.CustomerName + "','" +
                                                   cust.PersonalPhone + "','" + cust.OfficePhone + "','" +
                                                   cust.EmailId + "')";
                    contactDetailsCommand.ExecuteNonQuery();
                }
                cmd.Connection.Close();
            }
        }
    }
}