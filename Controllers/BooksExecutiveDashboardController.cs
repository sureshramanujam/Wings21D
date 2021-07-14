using System;
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
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();

            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Dashboard = new DataTable();
            DataTable UserIDTable = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(userName))
            {
                try
                {
                    string asatdt = DateTime.Parse(asAtDate).ToString("yyyy-MM-dd");

                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Select UserName from CompanyUsers_Table Where UserName='" + userName + "'";
                    cmd.Connection = con;
                    da.SelectCommand = cmd;
                    da.Fill(UserIDTable);

                    cmd.CommandText = "SELECT " +
                                      "(SELECT count(*) From CashCollections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "' And TransactionDate <= '" + asatdt + "') AS CashTransactions, " +
                                      "(SELECT sum(amount) From CashCollections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "' And TransactionDate <= '" + asatdt + "') AS CashAmount, " +
                                      "(SELECT count(*) From ChequeCollections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "' And TransactionDate <= '" + asatdt + "') AS ChequeTransactions, " +
                                      "(SELECT sum(amount) From ChequeCollections_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "' And TransactionDate <= '" + asatdt + "') AS ChequeAmount," +
                                      "(SELECT count(*) From Books_CustomersPendingSalesOrder_Desktop_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "' And OrderDate <= '" + asatdt + "') AS PendingOrders, " +
                                      "(SELECT sum(LineAmount) From Books_CustomersPendingSalesOrder_Desktop_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "' And OrderDate <= '" + asatdt + "') AS PendingOrderValue," +
                                      "(SELECT count(*) From Books_CustomersSalesInvoices_Desktop_Table Where Username = '" + UserIDTable.Rows[0][0].ToString() + "' And InvoiceDate <= '" + asatdt + "') AS SalesInvoiceCount, " +
                                      "(SELECT sum(LineAmount) From Books_CustomersSalesInvoices_Desktop_Table Where Username='" + UserIDTable.Rows[0][0].ToString() + "' And InvoiceDate <= '" + asatdt + "') AS SalesInvoiceValue ";

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
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
