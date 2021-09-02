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
    public class UsersController : ApiController
    {
        // GET api/values/5
        public HttpResponseMessage Get(string cName, string uName, string uPwd)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + cName + @";Data Source=localhost\SQLEXPRESS");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            try
            {
                DataTable usersDT = new DataTable();
                {
                    con.Open();
                    cmd.CommandText = "Select UserName,CustomerName,isRetailer from CompanyUsers_Table Where Username='" + uName + "' And UserPassword='" + uPwd + "' And ActiveStatus=1";
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    usersDT.TableName = "Users";
                    da.Fill(usersDT);
                    con.Close();
                }
                if (usersDT.Rows.Count > 0)
                {
                    {
                        DataTable tablesDT = new DataTable();
                        con.Open();
                        cmd.CommandText = string.Empty;
                        cmd.CommandText = "Select name from sys.tables";
                        SqlDataAdapter da1 = new SqlDataAdapter();
                        da1.SelectCommand = cmd;
                        tablesDT.TableName = "Tables";
                        da1.Fill(tablesDT);
                        con.Close();

                        if (tablesDT != null && tablesDT.Rows.Count > 0)
                        {
                            List<string> tablesCreated = new List<string>();
                            foreach (DataRow dr in tablesDT.Rows)
                            {
                                tablesCreated.Add(dr[0].ToString());
                            }
                            new TablesRequired().CreateTables(cName, tablesCreated);
                        }
                    }
                    var responseObject = new
                    {
                        Users = usersDT
                    };
                    var response = Request.CreateResponse(HttpStatusCode.OK, responseObject, MediaTypeHeaderValue.Parse("application/json"));
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid User Credentails");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to Login");
            }
        }

        // POST api/values
        public HttpResponseMessage Post(List<Users> usr)
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
                cmd.CommandText = "Select count(*) from CompanyUsers_Table";
                SqlDataAdapter usersList = new SqlDataAdapter();
                DataTable fetchedUsers = new DataTable();
                con.Open();
                usersList.SelectCommand = cmd;
                usersList.Fill(fetchedUsers);
                con.Close();

                if (Convert.ToInt32(fetchedUsers.Rows[0][0].ToString()) > 0)
                {
                    con.Open();
                    cmd.CommandText = "Delete from CompanyUsers_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                try
                {
                    con.Open();
                    foreach (Users users in usr)
                    {
                        cmd.CommandText = "Insert Into CompanyUsers_Table Values('" + users.userName + "','" + users.userPassword + "'," + users.userStatus +
                                          "," + users.isRetailer + ",'" + users.customerName + "')";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }

    }

    public class TablesRequired
    {
        public void CreateTables(string cName, List<string> tablescreated)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + cName + @";Data Source=localhost\SQLEXPRESS");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            List<string> tablesToCreate = GetTablesList();
            foreach (string name in tablesToCreate)
            {
                if (!tablescreated.Contains(name))
                {
                    cmd.CommandText = GetCommandText(name);
                    if (!string.IsNullOrEmpty(cmd.CommandText))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }
        List<string> GetTablesList()
        {
            return new List<string>
            {
                "CompanyDetails_Table",
                "Books_SalesInvoice_Payments_Table",
                "Books_Customer_Ledger_Table",
                "Books_SalesInvoice_Table"
            };
        }
        string GetCommandText(string name)
        {
            switch (name)
            {
                case "Books_SalesInvoice_Payments_Table":
                    return "Create Table Books_SalesInvoice_Payments_Table (" +
                       "DocumentNo nvarchar(200) null," +
                       "CashAmount Decimal(18,2) null," +
                       "ChequeAmount Decimal(18,2) null," +
                       "ChequeNumber nvarchar(100) null," +
                       "ChequeDate date null)";

                case "Books_Customer_Ledger_Table":
                    return "Create Table Books_Customer_Ledger_Table (" +
                        "TransactionType nvarchar(50) null," +
                        "VoucherNumber nvarchar(100) null," +
                        "VoucherDate date null," +
                        "Account nvarchar(265) null," +
                        "ContraAccount nvarchar(265) null," +
                        "DebitAmount decimal(18,2) null," +
                        "CreditAmount decimal(18,2) null," +
                        "BalanceAmount decimal(18,2) null," +
                        "Remarks nvarchar(265) null)";
                case "Books_SalesInvoice_Table":
                    return "Create Table Books_SalesInvoice_Table (" +
                            "TransactionNo nchar(100) null," +
                                              "TransactionDate date null," +
                                              "InvoiceType nchar(10) null," +
                                              "CashCreditType nchar(10) null," +
                                              "CustomerName nvarchar(265) null," +
                                              "ProductName nvarchar(265) null," +
                                              "Quantity decimal(18,2) null," +
                                              "TransactionRemarks nvarchar(265) null," +
                                              "BranchName nvarchar(265) null," +
                                              "LocationName nvarchar(265) null," +
                                              "DivisionName nvarchar(265) null," +
                                              "ProjectName nvarchar(265) null," +
                                              "TransactionSeries nchar(10) null," +
                                              "DocumentNo nvarchar(200) null," +
                                              "DownloadedFlag int null," +
                                              "Username nvarchar(265) null)";

            }
            return null;
        }
    }
}