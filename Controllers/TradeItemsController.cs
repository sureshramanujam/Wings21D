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
    public class TradeItemsController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        public HttpResponseMessage Get(string dbName, string pc)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Items = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    if(String.IsNullOrEmpty(pc))
                    {
                        if(pc=="All Profit Centers")
                        {
                            cmd.CommandText = "With ProductsList As ( " +
                                              "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                              "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                              "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                              "From Trade_Items_Table a " +
                                              "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                              "Where BalanceQty>0 " +
                                              "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                              ") Select * from ProductsList Where BalanceQty>0 And ProfitCenterName='" + pc + 
                                              "' Order By ItemName Order by ItemName";
                        }
                        else
                        {
                            cmd.CommandText = "With ProductsList As ( " +
                                      "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                      "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                      "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                      "From Trade_Items_Table a " +
                                      "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                      "Where BalanceQty>0 " +
                                      "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                      ") Select * from ProductsList Where BalanceQty > 0 Order By ItemName Order by ItemName";
                        }
                    }

                    

                    da.SelectCommand = cmd;
                    Items.TableName = "Items";
                    da.Fill(Items);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    Items = Items
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        // POST api/values
        public HttpResponseMessage Post(List<TradeItems> ti)
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

            /*
            //SqlDataAdapter da = new SqlDataAdapter();
            //DataTable dt = new DataTable();
            if (dt.Rows.Count > 0)
            {
                cmd.CommandText = "Delete from Trade_Items_Table Where ItemName='" + titems.itemName + "'";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Insert Into Trade_Items_Table Values(NEWID(),'" + titems.itemName + "', '" +
                titems.productName + "','" + titems.hsnsac + "','" + titems.profitCenterName + "'," + titems.rateperpiece +
                "," + titems.rateperpack + ",'" + titems.gstrate + "'," + titems.piecesperpack + "," + titems.itemmrp + titems.activeStatus + ")";
                cmd.ExecuteNonQuery();

                dt.Clear();

                cmd.CommandText = "Select count(*) from Trade_Items_Table Where ItemName = '" + titems.itemName + "'";
                da.Fill(dt);
            }
            */

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    foreach (TradeItems titems in ti)
                    {
                        cmd.CommandText = "Insert Into Trade_Items_Table Values(NEWID(),'" + titems.itemName + "', '" +
                        titems.productName + "','" + titems.hsnsac + "','" + titems.profitCenterName + "'," + titems.rateperpiece +
                        "," + titems.rateperpack + ",'" + titems.gstrate + "'," + titems.piecesperpack + "," + titems.itemmrp + "," + titems.activeStatus + ")";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                catch
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
    }
}
