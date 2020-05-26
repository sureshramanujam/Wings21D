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
    public class BooksCustomerLedgerController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string custName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable CustomerLedger = new DataTable();            

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(custName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    cmd.CommandText = "Select * from Books_Customer_Ledger_Table Where Account='" + custName + "' " + 
                                      "Order by VoucherDate, VoucherNumber";
                    da.SelectCommand = cmd;
                    CustomerLedger.TableName = "CustomerLedger";
                    da.Fill(CustomerLedger);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    CustomerLedger = CustomerLedger
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
                
            }
        }
      
        // POST api/<controller>                
        public HttpResponseMessage Post(List<BooksCustomerLedger> TCL)
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

                if(uploadAll.Trim().ToLower() == "true")
                {
                    cmd.CommandText = "Select name from sys.tables where name='Books_Customer_Ledger_Table'";
                    da.SelectCommand = cmd;
                    dt.Clear();
                    da.Fill(dt);
                    con.Close();

                    if (dt.Rows.Count > 0)
                    {
                        con.Open();
                        cmd.CommandText = "Delete From Books_Customer_Ledger_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();

                        con.Open();

                        foreach (BooksCustomerLedger a in TCL)
                        {
                            a.Account = a.Account.Replace("'", "''");
                            a.ContraAccount = a.ContraAccount.Replace("'", "''");
                            a.Remarks = a.Remarks.Replace("'", "''");
                            cmd.CommandText = "Insert Into Books_Customer_Ledger_Table Values('" + a.TransactionType + "','" + a.VoucherNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.VoucherDate) + "','" +
                                                     a.Account + "','" + a.ContraAccount + "'," + a.DebitAmount + "," + a.CreditAmount + "," + a.BalanceAmount + ",'" +
                                                    a.Remarks + "')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    else
                    {
                        try
                        {
                            con.Open();

                            cmd.CommandText = "Create Table Books_Customer_Ledger_Table (" +
                                              "TransactionType nvarchar(50) null," +
                                              "VoucherNumber nvarchar(100) null," +
                                              "VoucherDate date null," +
                                              "Account nvarchar(265) null," +
                                              "ContraAccount nvarchar(265) null," +
                                              "DebitAmount decimal(18,2) null," +
                                              "CreditAmount decimal(18,2) null," +
                                              "Balancemount decimal(18,2) null," +
                                              "Remarks nvarchar(265) null)";
                            cmd.ExecuteNonQuery();

                            foreach (BooksCustomerLedger a in TCL)
                            {
                                a.Account = a.Account.Replace("'", "''");
                                a.ContraAccount = a.ContraAccount.Replace("'", "''");
                                a.Remarks = a.Remarks.Replace("'", "''");
                                cmd.CommandText = "Insert Into Books_Customer_Ledger_Table Values('" + a.TransactionType + "','" + a.VoucherNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.VoucherDate) + "','" +
                                                  a.Account + "','" + a.ContraAccount + "'," + a.DebitAmount + "," + a.CreditAmount + "," + a.BalanceAmount + ",'" +
                                                 a.Remarks + "')";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            //return "Unable to insert data.";
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }
                        //return voucherNumbers;
                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                }
                else
                {
                    cmd.CommandText = "Select name from sys.tables where name='Books_Customer_Ledger_Table'";
                    da.SelectCommand = cmd;
                    dt.Clear();
                    da.Fill(dt);
                    con.Close();

                    if (dt.Rows.Count > 0)
                    {
                        con.Open();
                        foreach (BooksCustomerLedger a in TCL)
                        {
                            a.Account = a.Account.Replace("'", "''");
                            a.ContraAccount = a.ContraAccount.Replace("'", "''");
                            a.Remarks = a.Remarks.Replace("'", "''");
                            cmd.CommandText = "Insert Into Books_Customer_Ledger_Table Values('" + a.TransactionType + "','" + a.VoucherNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.VoucherDate) + "','" +
                                                     a.Account + "','" + a.ContraAccount + "'," + a.DebitAmount + "," + a.CreditAmount + "," + a.BalanceAmount + ",'" +
                                                    a.Remarks + "')";
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    else
                    {
                        try
                        {
                            con.Open();

                            cmd.CommandText = "Create Table Books_Customer_Ledger_Table (" +
                                              "TransactionType nvarchar(50) null," +
                                              "VoucherNumber nvarchar(100) null," +
                                              "VoucherDate date null," +
                                              "Account nvarchar(265) null," +
                                              "ContraAccount nvarchar(265) null," +
                                              "DebitAmount decimal(18,2) null," +
                                              "CreditAmount decimal(18,2) null," +
                                              "Balancemount decimal(18,2) null," +
                                              "Remarks nvarchar(265) null)";
                            cmd.ExecuteNonQuery();

                            foreach (BooksCustomerLedger a in TCL)
                            {
                                a.Account = a.Account.Replace("'", "''");
                                a.ContraAccount = a.ContraAccount.Replace("'", "''");
                                a.Remarks = a.Remarks.Replace("'", "''");
                                cmd.CommandText = "Insert Into Books_Customer_Ledger_Table Values('" + a.TransactionType + "','" + a.VoucherNumber + "','" + String.Format("{0:yyyy-MM-dd}", a.VoucherDate) + "','" +
                                                  a.Account + "','" + a.ContraAccount + "'," + a.DebitAmount + "," + a.CreditAmount + "," + a.BalanceAmount + ",'" +
                                                 a.Remarks + "')";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            //return "Unable to insert data.";
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }
                        //return voucherNumbers;
                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            //return voucherNumbers;
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
