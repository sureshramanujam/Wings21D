using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Wings21D.Controllers
{
    public class BooksCustomerSalesReturnsController : ApiController
    {
        // GET: API <Controllers>
        public HttpResponseMessage Get(string dbName ,string custName, string fromDate,string toDate)
        {
            if(String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(custName))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable SalesReturn = new DataTable();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                String frmDt = DateTime.Parse(fromDate).ToString("yyyy - MM - dd");
                String toDt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");

                cmd.CommandText = "";

                da.SelectCommand = cmd;
                SalesReturn.TableName = "SalesReturn";
                da.Fill(SalesReturn);
                con.Close();

            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            var returnResponseObject = new
            {
                SalesReturn = SalesReturn
            };
            var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
            return response;
        }
    }
}