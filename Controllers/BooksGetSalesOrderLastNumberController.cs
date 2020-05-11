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
    public class BooksGetSalesOrderLastNumberController : ApiController
    {

        // GET api/<controller>
        public string Get(string dbName, string userName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable LatestSalesOrder = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    cmd.CommandText = "Select Top 1 TransactionNo From Books_SalesOrder_Table Where Username='" + userName + "' Order by TransactionNo Desc";

                    da.SelectCommand = cmd;
                    LatestSalesOrder.TableName = "LatestSalesOrder";
                    da.Fill(LatestSalesOrder);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
                return LatestSalesOrder.Rows[0][0].ToString();
            }
            else
            {
                return "Error";
            }
        }
    }
}
