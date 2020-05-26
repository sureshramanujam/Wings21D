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
    public class TradeItemBalanceController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable ItemBalance = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Trade_ItemBalance_Table Order By ItemName", con);
                    da.SelectCommand = cmd;
                    ItemBalance.TableName = "ItemBalance";
                    da.Fill(ItemBalance);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    ItemBalance = ItemBalance
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        // GET api/values/5

        // POST api/values
        public HttpResponseMessage Post(List<TradeItemBalance> itembalance)
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

            /*
            con.Open();
            SqlDataAdapter itemBalancesAdapter = new SqlDataAdapter();
            DataTable availableItemBalances = new DataTable();
            cmd.CommandText = "Select * From Trade_ItemBalance_table";
            itemBalancesAdapter.SelectCommand = cmd;
            itemBalancesAdapter.Fill(availableItemBalances);
            */

            //con.Close();

            if (!String.IsNullOrEmpty(dbName))
            {
                if (uploadAll.ToLower().Trim() == "true")
                {   
                    try
                    {
                        //if (availableItemBalances.Rows.Count > 0)
                        //{
                        con.Open();
                        cmd.CommandText = "Delete from Trade_ItemBalance_Table";
                        cmd.ExecuteNonQuery();
                        con.Close();
                        //}

                        con.Open();
                        foreach (TradeItemBalance tib in itembalance)
                        {
                            tib.itemName = tib.itemName.Replace("'", "''");
                            cmd.CommandText = "Insert Into Trade_ItemBalance_Table Values('" + tib.itemName + "', '" +
                                              tib.locationName + "'," + tib.availableQtyInPieces + ")";
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
                    con.Open();

                    foreach (TradeItemBalance tib in itembalance)
                    {
                        tib.itemName = tib.itemName.Replace("'", "''");
                        cmd.CommandText = "Insert Into Trade_ItemBalance_Table Values('" + tib.itemName + "', '" +
                                          tib.locationName + "'," + tib.availableQtyInPieces + ")";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();

                    return new HttpResponseMessage(HttpStatusCode.Created);
                }
                //return new HttpResponseMessage(HttpStatusCode.Created);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
