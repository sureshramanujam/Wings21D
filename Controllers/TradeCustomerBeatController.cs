using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;


namespace Wings21D.Controllers
{
    public class TradeCustomerBeatController : ApiController
    {
        // GET: TradeCustomerBeat
        public HttpResponseMessage Get(string dbName, string customerName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter();
                con.Open();
                cmd.CommandText = "Select BeatName from Trade_Customers_Table where CustomerName='" + customerName + "' ";
                sda.SelectCommand = cmd;
                sda.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    var responseObject = new
                    {
                        Beats = dt
                    };
                    var response = Request.CreateResponse(HttpStatusCode.OK, responseObject, MediaTypeHeaderValue.Parse("application/json"));
                    return response;
                }
                else
                {
                    var response = Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Data Found");
                    return response;
                }

            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}