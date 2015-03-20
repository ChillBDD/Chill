namespace Chill.ExampleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web;
    using System.Web.Http;
    using System.Web.Security;


    public class TestController : ApiController
    {
        [HttpGet]
        [Route("test")]
        public string Get()
        {
            return "Tested Get";
        }

        [HttpPost]
        [Route("test")]
        public string Post()
        {
            return "Tested Post";
        }


        [Authorize]
        [HttpGet]
        [Route("RequiresAuthentication")]
        public string RequiresAuthenticationGet()
        {
            return "Ok";
        }


        /// <summary>
        /// This is a horrible way to log in.. 
        /// </summary>
        [HttpPost]
        [Route("LogIn")]
        public HttpResponseMessage LogIn()
        {
            try
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "test", DateTime.Now, DateTime.Now.AddMinutes(30), true, null, FormsAuthentication.FormsCookiePath);
                string encTicket = FormsAuthentication.Encrypt(ticket);
                
                response.Headers.AddCookies(new []{new CookieHeaderValue(FormsAuthentication.FormsCookieName, encTicket)});

                return response;
                //this.ActionContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                //FormsAuthentication.SetAuthCookie("user", false);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to log in: " + ex.ToString());
                throw;
            }
        }
    }



    
}
