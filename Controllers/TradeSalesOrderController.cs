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
    public class TradeSalesOrderController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string asAtDate)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesOrders = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    String beatName = "BeatName";

                    SqlCommand sqlCommand = new SqlCommand();
                    DataTable dataTable = new DataTable();
                    sqlCommand.CommandText = "SELECT COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='Trade_SalesOrder_Table'";
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlCommand.Connection = con;
                    con.Open();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    sqlDataAdapter.Fill(dataTable);
                    con.Close();

                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    DateTime asonDate = DateTime.Parse(asAtDate);
                    if (dataTable.AsEnumerable().Any(row => beatName == row.Field<String>("COLUMN_NAME")) && dataTable.Rows.Count > 0)
                    {
                        cmd.CommandText = "select DocumentNo, Convert(varchar,TransactionDate,112) as TransactionDate, CustomerName, BeatName, ProfitCenteRname, " +
                                      "ItemName, QuantityInPieces, QuantityInPacks, TransactionRemarks, Username from Trade_SalesOrder_Table  Where " +
                                      "convert(varchar,TransactionDate,105) <= '" + asonDate.ToString() +
                                      "' And DownloadedFlag=0 Order By TransactionNo";
                    }
                    else
                    {
                        cmd.CommandText = "select a.DocumentNo, Convert(varchar,a.TransactionDate,112) as TransactionDate, a.CustomerName, b.BeatName, a.ProfitCenteRname, " +
                                      "a.ItemName, a.QuantityInPieces, a.QuantityInPacks, a.TransactionRemarks, a.Username from Trade_SalesOrder_Table a, Trade_Customers_Table b Where " +
                                      "a.CustomerName=b.CustomerName and convert(varchar,a.TransactionDate,105) <= '" + asonDate.ToString() +
                                      "' And a.DownloadedFlag=0 Order By a.TransactionNo";
                    }
                    
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
        [HttpPost]
        public HttpResponseMessage SaveOrder(List<SalesOrderEntry> mySO)
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
                
                    try
                    {
                    SqlCommand sqlCommand = new SqlCommand();
                    DataTable dataTable = new DataTable();
                    sqlCommand.CommandText = "SELECT COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='Trade_SalesOrder_Table'";
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                    sqlCommand.Connection = con;
                    con.Open();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    sqlDataAdapter.Fill(dataTable);
                    con.Close();
                    String beatName = "BeatName";
                       if ( !dataTable.AsEnumerable().Any(row => beatName == row.Field<String>("COLUMN_NAME")) && dataTable.Rows.Count>0)
                       {
                        con.Open();
                        sqlCommand.CommandText = "Alter Table Trade_SalesOrder_Table Add BeatName varchar(100)";
                        sqlCommand.ExecuteNonQuery();
                        con.Close();
                       }

                    }
                    catch (Exception ex)
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                
                try
                {
                    con.Open();

                    //cmd.CommandText = "Select ISNULL(Max(TransactionNo), 0) + 1 From Trade_SalesOrder_Table Where YEAR(convert(varchar,TransactionDate,23)) = '" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "'";
                    cmd.CommandText = "Select ISNULL(Max(TransactionNo), 0) + 1 From Trade_SalesOrder_Table";
                    SqlDataAdapter docNumberAdapter = new SqlDataAdapter();
                    DataTable newDocumentNumber = new DataTable();
                    docNumberAdapter.SelectCommand = cmd;
                    docNumberAdapter.Fill(newDocumentNumber);

                    String sProfitCenter = String.Empty;
                    string transRemarks = String.Empty;

                    foreach (SalesOrderEntry soe in mySO)
                    {
                        if (soe.profitCenterName == "All Profit Centers")
                            sProfitCenter = "Profit Center";
                        else
                            sProfitCenter = soe.profitCenterName;

                        transRemarks = soe.transactionRemarks.Replace("\\n", "");
                        soe.itemName = soe.itemName.Replace("'", "''");
                        soe.customerName = soe.customerName.Replace("'", "''");

                        cmd.CommandText = "Insert Into Trade_SalesOrder_Table Values(" + Convert.ToInt32(newDocumentNumber.Rows[0][0]) +
                                          ",'" + String.Format("{0:yyyy-MM-dd}", todayDate.Date) + "','" + soe.customerName + "', '" + soe.itemName + "'," +
                                          soe.quantityInPieces + "," + soe.quantityInPacks + ",'" + transRemarks + "','OR-M-'," +
                                          "'OR-M-" + Convert.ToInt32(newDocumentNumber.Rows[0][0]).ToString() +  "',0,'" + soe.userName + "','" +
                                          sProfitCenter + "','"+soe.beatName+"')";

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
