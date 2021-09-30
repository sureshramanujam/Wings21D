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
using System.Web.Mvc;

namespace Wings21D.Controllers.CommonControllers
{
    public class SeverReleaseDateController : ApiController
    {
        // GET<API> SeverReleaseDate

        public HttpResponseMessage Get()
        {

            try
            {
                string releaseDate = "16-Sep-2021";
                return Request.CreateResponse(HttpStatusCode.OK, releaseDate, MediaTypeHeaderValue.Parse("application/json"));

            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

           

           // var response = Request.CreateResponse(HttpStatusCode.OK, returnResponseObject, MediaTypeHeaderValue.Parse("application/json"));
            //return response;
        }

    }

}
