﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Wings21D.Controllers
{
    public class BooksExecutiveDashboardController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName, string userName, string asAtDate)
        {
            if (String.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(asAtDate) || string.IsNullOrEmpty(userName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid parameters.");
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();

            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Dashboard = new DataTable();
            DataTable UserIDTable = new DataTable();

           
                try
                {
                  //  string asatdt = DateTime.Parse(asAtDate).ToString("yyyy-MM-dd");

                    string[] asatdates = asAtDate.Split('-');
                    string asatdt = asatdates[2] + "-" + asatdates[1] + "-" + asatdates[0];

                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Select UserName from CompanyUsers_Table Where UserName='" + userName + "'";
                    cmd.Connection = con;
                    da.SelectCommand = cmd;
                    da.Fill(UserIDTable);

                    string username = string.Empty;
                    if(UserIDTable!=null && UserIDTable.Rows.Count > 0)
                    {
                        username = UserIDTable.Rows[0][0].ToString();
                    }
                    cmd.CommandText = "SELECT " +
                                      "(SELECT ISNULL(count(*),0) From CashCollections_Table Where Username='" + username + "' And TransactionDate <= '" + asatdt + "') AS CashTransactions, " +
                                      "(SELECT ISNULL(sum(amount),0) From CashCollections_Table Where Username='" + username + "' And TransactionDate <= '" + asatdt + "') AS CashAmount, " +
                                      "(SELECT ISNULL(count(*),0) From ChequeCollections_Table Where Username='" + username + "' And TransactionDate <= '" + asatdt + "') AS ChequeTransactions, " +
                                      "(SELECT ISNULL(sum(amount),0) From ChequeCollections_Table Where Username='" + username + "' And TransactionDate <= '" + asatdt + "') AS ChequeAmount," +
                                      "(Select ISNULL(sum(a.Quantity * b.SalesPrice),0) from Books_SalesOrder_Table a left join Books_Products_Table b on a.ProductName=b.ProductName Where a.Username='" + username + "' And a.TransactionDate <= '" + asatdt + "') AS PendingOrderValue," +
                                      "(Select ISNULL(Sum(b.PendingValue),0) From Books_Customers_Table a LEFT Join Books_CustomersPendingBills_Table b On a.CustomerName = b.CustomerName ) As Receivables,"+
                                      "(Select ISNULL(sum(a.Quantity * b.SalesPrice),0) from Books_SalesInvoice_Table a left join Books_Products_Table b on a.ProductName=b.ProductName Where a.Username='Supervisor' And a.TransactionDate <= '2021-09-02') AS SalesInvoiceValue ";

                    da.SelectCommand = cmd;
                    Dashboard.TableName = "Dashboard";
                    da.Fill(Dashboard);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Dashboard = Dashboard
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            
        }
    }
}
