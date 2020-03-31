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
    public class TradeCashCollectionsReportController : ApiController
    {

        // GET api/<controller>
        public HttpResponseMessage Get(string dbName, string userName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + dbName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();
            List<string> mn = new List<string>();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable CashCollections = new DataTable();

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    //DateTime asonDate = DateTime.Parse(asAtDate);

                    cmd.CommandText = "select DocumentNo, Format(TransactionDate,'dd-MMM-yyyy') As 'CollectionDate' From CashCollections_Table " +
                                      //"Where Convert(varchar,a.TransactionDate,23) <= '" + asonDate.ToString() + "' " +
                                      "Where Username='" + userName + "' " +
                                      "Group by DocumentNo, TransactionDate " +
                                      "Order By CollectionDate Desc, DocumentNo";

                    da.SelectCommand = cmd;
                    CashCollections.TableName = "CashCollections";
                    da.Fill(CashCollections);
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                var returnResponseObject = new
                {
                    CashCollections = CashCollections
                };

                var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
                
            }
        }
    }
}
