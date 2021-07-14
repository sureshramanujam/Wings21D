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
    public class UpdateTransactionFlagController : ApiController
    {
        // POST api/<controller>                
        [HttpPost]
        public HttpResponseMessage Post(List<DocumentNumbers> voucherNumbers)
        {
            var re = Request;
            var headers = re.Headers;
            String dbName = String.Empty;
            String transactionType = String.Empty;

            if (headers.Contains("dbname"))
                dbName = headers.GetValues("dbname").First();

            if (headers.Contains("transactiontype"))
                transactionType = headers.GetValues("transactiontype").First();

            if (String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(transactionType))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                try
                {
                    con.Open();
                    if (transactionType == "salesorder")
                    {
                        foreach (DocumentNumbers vno in voucherNumbers)
                        {
                            cmd.CommandText = "Update Trade_SalesOrder_Table Set DownloadedFlag=1 Where DocumentNo='" + vno.documentNo + "'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "Update Books_SalesOrder_Table Set DownloadedFlag=1 Where DocumentNo='" + vno.documentNo + "'";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        foreach (DocumentNumbers vno in voucherNumbers)
                        {
                            String tableName = String.Empty;
                            switch (transactionType)
                            {
                                case "cbcollection": tableName = "Collections_Table"; break;
                                case "cashcollection": tableName = "CashCollections_Table"; break;
                                case "chequecollection": tableName = "ChequeCollections_Table"; break;
                                case "salesinvoice": tableName = "Books_SalesInvoice_Table"; break;
                            }
                            cmd.CommandText = "Update " + tableName + " Set DownloadedFlag=1 Where DocumentNo='" + vno.documentNo + "'";
                            cmd.ExecuteNonQuery();

                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
