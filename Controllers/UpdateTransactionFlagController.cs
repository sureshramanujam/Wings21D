﻿using System;
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

            if(headers.Contains("transactiontype"))
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

                    switch (transactionType)
                    {
                        case "cbcollection":
                            foreach (DocumentNumbers vno in voucherNumbers)
                            {
                                cmd.CommandText = "Update Collections_Table Set DownloadedFlag=1 Where DocumentNo='" + vno.documentNo + "'";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                            break;
                        case "cashcollection":
                            foreach (DocumentNumbers vno in voucherNumbers)
                            {
                                cmd.CommandText = "Update CashCollections_Table Set DownloadedFlag=1 Where DocumentNo='" + vno.documentNo + "'";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                            break;
                        case "chequecollection":
                            foreach (DocumentNumbers vno in voucherNumbers)
                            {
                                cmd.CommandText = "Update ChequeCollections_Table Set DownloadedFlag=1 Where DocumentNo='" + vno.documentNo + "'";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                            break;
                        case "salesorder":
                            foreach (DocumentNumbers vno in voucherNumbers)
                            {
                                cmd.CommandText = "Update Trade_SalesOrder_Table Set DownloadedFlag=1 Where DocumentNo='" + vno.documentNo + "'";
                                cmd.ExecuteNonQuery();
                            }
                            con.Close();
                            break;
                    }
                }
                catch (Exception ex)
                { 
                    return new HttpResponseMessage(HttpStatusCode.NotFound); 
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Created);        
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
