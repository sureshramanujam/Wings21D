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
        public HttpResponseMessage Get(string dbName, string pc, string zerostock,string categoryName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable Items = new DataTable();

            if (!String.IsNullOrEmpty(dbName) && !String.IsNullOrEmpty(pc))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    
                        if(pc.ToUpper()!="ALL PROFIT CENTERS")
                        {
                            if (zerostock == "true")
                            {
                                    if (categoryName==null||categoryName.Equals(string.Empty)||categoryName.ToUpper()=="All Categories")
                                    {
                                        cmd.CommandText = "With ProductsList As ( " +
                                                          "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                                          "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                                          "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                                          "From Trade_Items_Table a " +
                                                          "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                                          "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                                          ") Select * from ProductsList Where ProfitCenterName='" + pc +
                                                          "' Order By ItemName";

                                    }
                                    else
                                    {
                                        cmd.CommandText = "With ProductsList As ( " +
                                                          "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                                          "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                                          "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                                          "From Trade_Items_Table a " +
                                                          "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                                          "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                                          ") Select * from ProductsList left join Trade_ProductCategories_Table on ProductsList.ItemName=Trade_ProductCategories_Table.ProductName" +
                                                          " where Trade_ProductCategories_Table.CategoryName='" + categoryName + "' and ProductsList.ProfitCenterName='" + pc +
                                                          "' Order By ProductsList.ItemName";

                                     }
                                
                            }
                            else
                            {
                                    if (categoryName == null || categoryName.Equals(string.Empty) || categoryName.ToUpper() == "All Categories")
                                    {
                                          cmd.CommandText = "With ProductsList As ( " +
                                                  "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                                  "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                                  "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                                  "From Trade_Items_Table a " +
                                                  "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                                  "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                                  ") Select * from ProductsList Where BalanceQty>0 And ProfitCenterName='" + pc +
                                                  "' Order By ItemName";

                                    }
                                    else
                                    {
                                          cmd.CommandText = "With ProductsList As ( " +
                                                  "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                                  "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                                  "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                                  "From Trade_Items_Table a " +
                                                  "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                                  "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                                  ") Select * from ProductsList left join Trade_ProductCategories_Table on ProductsList.ItemName=Trade_ProductCategories_Table.ProductName" +
                                                  " where Trade_ProductCategories_Table.CategoryName='"+categoryName+ "' and ProductsList.BalanceQty>0 And ProductsList.ProfitCenterName='" + pc +
                                                  "' Order By ProductsList.ItemName";

                                    }
                                
                            }
                        }
                        else
                        {
                            if (zerostock == "true")
                            {

                                    if (categoryName == null || categoryName.Equals(string.Empty) || categoryName.ToUpper() == "All Categories")
                                    {
                                      cmd.CommandText = "With ProductsList As ( " +
                                       "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                       "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                       "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                       "From Trade_Items_Table a " +
                                       "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                       "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                       ") Select * from ProductsList Order By ItemName";

                                    }
                                    else
                                    {
                                        cmd.CommandText = "With ProductsList As ( " +
                                           "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                           "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                           "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                           "From Trade_Items_Table a " +
                                           "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                           "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                           ") Select * from ProductsList left join Trade_ProductCategories_Table on ProductsList.ItemName=Trade_ProductCategories_Table.ProductName" +
                                           " where Trade_ProductCategories_Table.CategoryName='"+categoryName+ "' Order By ProductsList.ItemName";
                                   }
                                
                            }
                            else
                            {
                                    if (categoryName == null || categoryName.Equals(string.Empty) || categoryName.ToUpper() == "All Categories")
                                    {
                                        cmd.CommandText = "With ProductsList As ( " +
                                                  "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                                  "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                                  "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                                  "From Trade_Items_Table a " +
                                                  "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                                  "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                                  ") Select * from ProductsList Where BalanceQty>0 Order By ItemName";

                                    }
                                    else
                                    {
                                        cmd.CommandText = "With ProductsList As ( " +
                                                  "Select a.ItemName, a.ProductName, a.HSNSAC,  a.ProfitCenterName, a.GSTRate," +
                                                  "Sum(a.RatePerPiece)RatePerPiece, Sum(a.RatePerPack)RatePerPack, Sum(a.ItemMRP) ItemMRP, " +
                                                  "ISNULL(Sum(b.AvailableQtyInPieces),0) BalanceQty " +
                                                  "From Trade_Items_Table a " +
                                                  "Left Join Trade_ItemBalance_Table b On a.ItemName=b.ItemName " +
                                                  "Group by a.ItemName, a.ProductName, a.ProfitCenterName, a.HSNSAC, a.GSTRate, b.ItemName " +
                                                  ") Select * from ProductsList left join Trade_ProductCategories_Table on ProductsList.ItemName=Trade_ProductCategories_Table.ProductName " +
                                                  "where Trade_ProductCategories_Table.CategoryName='"+categoryName+ "' and ProductsList.BalanceQty>0 Order By ProductsList.ItemName";

                                    }
                                
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
                if (!String.IsNullOrEmpty(uploadAll))
                {
                    if (uploadAll.Trim().ToLower() == "true")
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandText = "Delete from Trade_Items_Table";
                            cmd.ExecuteNonQuery();
                            con.Close();
                            string itemName = String.Empty;

                            con.Open();
                            foreach (TradeItems titems in ti)
                            {
                                try
                                {
                                    titems.itemName = titems.itemName.Replace("'", "''");
                                    titems.productName = titems.productName.Replace("'", "''");
                                    cmd.CommandText = "Insert Into Trade_Items_Table Values(NEWID(),'" + titems.itemName + "', '" +
                                    titems.productName + "','" + titems.hsnsac + "','" + titems.profitCenterName + "'," + titems.rateperpiece +
                                    //cmd.CommandText = "Insert Into Trade_Items_Table Values(NEWID(),'" + titems.itemName + "', NULL," +
                                    //"'" + titems.hsnsac + "','" + titems.profitCenterName + "'," + titems.rateperpiece +
                                   "," + titems.rateperpack + ",'" + titems.gstrate + "'," + titems.piecesperpack + "," + titems.itemmrp + "," + titems.activeStatus + ")";
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    var response = Request.CreateResponse(HttpStatusCode.InternalServerError, titems.itemName + ex.ToString(), MediaTypeHeaderValue.Parse("application/json"));
                                    return response;
                                }
                            }
                            con.Close();
                        }
                        catch (Exception e)
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            //var response = Request.CreateResponse(HttpStatusCode.InternalServerError, tite, MediaTypeHeaderValue.Parse("application/json"));
                            //return response;
                        }
                        return new HttpResponseMessage(HttpStatusCode.Created);
                    }
                    else
                    {
                        try
                        {
                            con.Open();
                            foreach (TradeItems titems in ti)
                            {
                                titems.itemName = titems.itemName.Replace("'", "''");
                                titems.productName = titems.productName.Replace("'", "''");
                                cmd.CommandText = "Insert Into Trade_Items_Table Values(NEWID(),'" + titems.itemName + "', '" +
                                titems.productName + "','" + titems.hsnsac + "','" + titems.profitCenterName + "'," + titems.rateperpiece +
                                //cmd.CommandText = "Insert Into Trade_Items_Table Values(NEWID(),'" + titems.itemName + "', NULL," +
                                //"'" + titems.hsnsac + "','" + titems.profitCenterName + "'," + titems.rateperpiece +
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
