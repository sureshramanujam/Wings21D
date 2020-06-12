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
    public class UsersController : ApiController
    {
        // GET api/values/5
        public bool Get(string cName, string uName, string uPwd)
        {
            SqlConnection con = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + cName + @";Data Source=localhost\SQLEXPRESS");
            DataSet ds = new DataSet();

            string qStr = "Select Count(*) from CompanyUsers_Table Where Username='" + uName + "' And UserPassword='" + uPwd + "' And ActiveStatus=1";
            SqlCommand cmd = new SqlCommand(qStr, con);

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);

            if (Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // POST api/values
        public HttpResponseMessage Post(List<Users> usr)
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
            
            if (!String.IsNullOrEmpty(dbName))
            {
                cmd.CommandText = "Select count(*) from CompanyUsers_Table";
                SqlDataAdapter usersList = new SqlDataAdapter();
                DataTable fetchedUsers = new DataTable();
                con.Open();
                usersList.SelectCommand = cmd;
                usersList.Fill(fetchedUsers);
                con.Close();

                if(Convert.ToInt32(fetchedUsers.Rows[0][0].ToString()) > 0)
                {
                    con.Open();
                    cmd.CommandText = "Delete from CompanyUsers_Table";
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                try
                {
                    con.Open();
                    foreach (Users users in usr)
                    {
                        cmd.CommandText = "Insert Into CompanyUsers_Table Values('" + users.userName + "','" + users.userPassword + "'," + users.userStatus +  
                                          "," + users.isRetailer + ",'" + users.customerName + "')";
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }

    }
}
