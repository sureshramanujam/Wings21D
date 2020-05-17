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
    public class TradeBeatCollectionsDashboardController : ApiController
    {   
        public HttpResponseMessage Get(string dbName, string userName, string asAtDate)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable BeatCollections = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    
                    cmd.CommandText = "With CollectionsList As(" +
                                            "Select b.BeatName, Sum(a.Amount) As 'Amount' " +
                                            "From CashCollections_Table a Left Join Trade_Customers_Table b on a.CustomerName=b.CustomerName " +
                                            "Where Convert(varchar,TransactionDate,105) <= '" + asAtDate + "' And Username='" + userName + "' " +
                                            "Group by b.BeatName " +
                                            "Union " +
                                            "Select b.BeatName, Sum(a.Amount) As 'Amount' " +
                                            "From ChequeCollections_Table a Left Join Trade_Customers_Table b on a.CustomerName=b.CustomerName " +                                            
                                            "Where Convert(varchar,TransactionDate,105) <= '" + asAtDate + "' And Username='" + userName + "' " +
                                            "Group by b.BeatName " +
                                            "Union " +
                                            "Select b.BeatName, (Sum(a.CashAmount)+Sum(a.ChequeAmount)) As 'Amount' " +
                                            "From Collections_Table a Left Join Trade_Customers_Table b on a.CustomerName=b.CustomerName " +
                                            "Where Convert(varchar,TransactionDate,105) <= '" + asAtDate + "' And Username='" + userName + "' " +
                                            "Group by b.BeatName " +
                                      ") Select BeatName, Sum(Amount) As 'CollectionAmount' from CollectionsList Group By BeatName";

                    da.SelectCommand = cmd;
                    BeatCollections.TableName = "BeatCollections";
                    da.Fill(BeatCollections);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    BeatCollections = BeatCollections
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
