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
    public class BooksCustomersReceiptsController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string custName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesOrders = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(custName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    cmd.CommandText = "Select * from Books_CustomersReceipts_Desktop_Table Where CustomerName='" + custName + "' " + 
                                      "Order by VoucherDate, VoucherNumber";
                    da.SelectCommand = cmd;
                    SalesOrders.TableName = "SalesOrders";
                    da.Fill(SalesOrders);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    SalesOrders = SalesOrders
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
        public HttpResponseMessage Post(List<BooksCustomersReceipts> CSR)
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

            DateTime todayDate = DateTime.Now;
            var dateOnly = todayDate.Date;

            if (!String.IsNullOrEmpty(dbName))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd.CommandText = "Select * From Books_CustomersReceipts_Desktop_Table";
                da.SelectCommand = cmd;
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 0)
                {
                    cmd.CommandText = "Delete From Books_CustomersReceipts_Desktop_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    try
                    {
                        con.Open();

                        cmd.CommandText = "Select name from sys.tables where name='Books_CustomersReceipts_Desktop_Table'";
                        da.SelectCommand = cmd;
                        dt.Clear();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            cmd.CommandText = "Create Table Books_CustomersReceipts_Desktop_Table (" +
                                               "CustomerName nvarchar(265) null," +
                                               "VoucherNumber nchar(100) null," +
                                               "VoucherDate date null," +
                                               "PaymentMode nchar(25) null," +
                                               "ChequeNumber nchar(25) null," +
                                               "AgainstInvoiceNumber nchar(100) null," +
                                               "NetAmount decimal(18,2) null," +
                                               "Username nvarchar(265) null)";
                        }
                        else
                        {
                            foreach (BooksCustomersReceipts a in CSR)
                            {
                                cmd.CommandText = "Insert Into Books_CustomersReceipts_Desktop_Table Values('" + a.account + "','" + a.voucherno +
                                                  "','" + String.Format("{0:yyyy-MM-dd}", a.voucherdate) + "','" + a.paymentmode + "','" +
                                                  a.chequeno + "','" + a.againstinvno + "'," + a.netamount + ",'" + a.userName + "')";

                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
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

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
