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
    public class TradeGetSalesOrderValueController : ApiController
    {   
        public string Get(string dbName, string userName, string asAtDate, string salesExecutive)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesOrders = new DataTable();
            //DateTime asonDate = Convert.ToDateTime(asAtDate);
            string asOnDate = DateTime.Parse(asAtDate).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(salesExecutive))
            {
                salesExecutive = salesExecutive.Remove(salesExecutive.Length - 1, 1);
            }
            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    if (userName.ToUpper()=="SUPERVISOR")
                    {
                        if (string.IsNullOrEmpty(salesExecutive) || salesExecutive.ToUpper().Contains("SUPERVISOR"))
                        {
                            cmd.CommandText = "With SalesOrdersList As (" +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces * b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks * b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName = b.ItemName " +
                                      ")Select Sum(AmtPcs+AmtPacks) As 'OrderValue' From SalesOrdersList Where OrderDate <= '" + asOnDate + "'";
                                      
                        }
                        else
                        {
                            cmd.CommandText = "With SalesOrdersList As (" +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces * b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks * b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName = b.ItemName " +
                                      ")Select Sum(AmtPcs+AmtPacks) As 'OrderValue' From SalesOrdersList Where Username in (" + salesExecutive + ") " +
                                      "And OrderDate <= '" + asOnDate + "'";
                        }

                    }
                    else
                    {
                        cmd.CommandText = "With SalesOrdersList As (" +
                                        "Select a.DocumentNo, TransactionDate As 'OrderDate', " +
                                        "a.CustomerName, a.ItemName, b.RatePerPiece, b.RatePerPack, a.QuantityInPieces, a.QuantityInPacks, " +
                                        "(a.QuantityInPieces * b.RatePerPiece) As 'AmtPcs', (a.QuantityInPacks * b.RatePerPack) As 'AmtPacks', " +
                                        "TransactionRemarks, DownloadedFlag, a.Username " +
                                        "From Trade_SalesOrder_Table a " +
                                        "Left Join Trade_Items_Table b on a.ItemName = b.ItemName " +
                                      ")Select Sum(AmtPcs+AmtPacks) As 'OrderValue' From SalesOrdersList Where Username = '" + userName + "' " +
                                      "And OrderDate <= '" + asOnDate + "'";

                    }
                    
                    da.SelectCommand = cmd;
                    SalesOrders.TableName = "SalesOrders";
                    da.Fill(SalesOrders);
                    con.Close();

                    return SalesOrders.Rows[0][0].ToString();
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
                //return SalesOrders.Rows[0][0].ToString();
            }
            else
            {
                return "Database not found.";
            }
        }
    }
}
