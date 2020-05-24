using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;

namespace Wings21D.Controllers
{
    public class CreateDatabaseController : ApiController
    {
        // POST api/<controller>
        [HttpPost]
        public HttpResponseMessage CreateDatabase(string dbName)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Data Source=localhost\SQLEXPRESS");

            String cmdString = "RESTORE DATABASE " + dbName +                               
                               @" FROM DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\Backup\wings21d.bak' " +
                               @"WITH MOVE 'wings21d' TO N'D:\21DMobileData\" + dbName + ".mdf', " +
                               @"MOVE 'wings21d_log' TO N'D:\21DMobileData\" + dbName + "_log.ldf'";
                               //@"WITH MOVE 'wings21d' TO N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\Data\" + dbName + ".mdf', " +
                               //@"MOVE 'wings21d_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\Data\" + dbName + "_log.ldf'";
            SqlCommand cmd = new SqlCommand(cmdString, con);

            string responseMessage = String.Empty;

            if (!String.IsNullOrEmpty(dbName))
            {
                try
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                catch (SqlException ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
