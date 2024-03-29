﻿using System;
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
    public class BooksCustomerSalesOrdersDesktopController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string custName,string fromDate, string toDate)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
  //        DataSet ds = new DataSet();
  //        List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable DesktopSalesOrders = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(custName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    string fromDt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
                    string toDt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");

                    cmd.CommandText = "Select OrderNo,Convert(varchar,OrderDate,23) as OrderDate,Convert(varchar,DueDate,23) as DueDate," +
                        "CustomerName,Product,BookedQty,LineAmount,Username from Books_CustomersSalesOrdersBooked_Desktop_Table " +
                        "Where OrderDate between '" + fromDt + "' and '" + toDt + "' and CustomerName='" + custName + "' " +
                        "Group by OrderNo,OrderDate,DueDate,CustomerName,Product,BookedQty,LineAmount,Username " +
                        "Order by OrderNo,OrderDate";
                                      
                    da.SelectCommand = cmd;
                    DesktopSalesOrders.TableName = "DesktopSalesOrders";
                    da.Fill(DesktopSalesOrders);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    DesktopSalesOrders = DesktopSalesOrders
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>                
        public string Post(List<BooksCustomersSalesOrdersDesktop> CSOB)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            String uploadAll = String.Empty;
            bool uploadAllData = false;

            if (headers.Contains("dbname"))
            {
                dbName = headers.GetValues("dbname").First();
            }
            if (headers.Contains("uploadall"))
            {
                uploadAll = headers.GetValues("uploadall").First();
                uploadAllData = uploadAll == "true" ? true : false;
            }

            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            DateTime todayDate = DateTime.Now;
            var dateOnly = todayDate.Date;

            if (!String.IsNullOrEmpty(dbName))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd.CommandText = "Select name from sys.tables where name='Books_CustomersSalesOrdersBooked_Desktop_Table'";
                da.SelectCommand = cmd;
                dt.Clear();
                da.Fill(dt);
                con.Close();

                string orderNumbers = String.Empty;

                if (dt.Rows.Count == 0)
                {
                    con.Open();
                    cmd.CommandText = "Create Table Books_CustomersSalesOrdersBooked_Desktop_Table (" +
                                          "OrderNumber nchar(100) null," +
                                          "OrderDate date null," +
                                          "DueDate date null," +
                                          "CustomerName nvarchar(265) null," +
                                          "ProductName nvarchar(265) null," +
                                          "BookedQty decimal(18,2) null," +
                                          "LineAmount decimal(18,2) null," +
                                          "Username nvarchar(265) null)";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                {
                    if (uploadAllData)
                    {
                        con.Open();
                        cmd.CommandText = "Delete From Books_CustomersSalesOrdersBooked_Desktop_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                con.Open();

                foreach (BooksCustomersSalesOrdersDesktop a in CSOB)
                {
                    cmd.CommandText = "Insert Into Books_CustomersSalesOrdersBooked_Desktop_Table Values('" + a.OrderNumber + "','" +
                                      String.Format("{0:yyyy-MM-dd}", a.OrderDate) + "','" + String.Format("{0:yyyy-MM-dd}", a.DueDate) + "','" +
                                      a.CustomerName + "','" + a.ProductName + "'," + a.BookedQuantity + "," + a.LineAmount + ",'" + a.Username + "')";

                    cmd.ExecuteNonQuery();
                    orderNumbers += a.OrderNumber + "$";
                    orderNumbers = orderNumbers.Replace('\\', ' ');
                }
                con.Close();
                return orderNumbers;
            }
            else
            {
                return "Database not found.";
            }
        }
    }
}