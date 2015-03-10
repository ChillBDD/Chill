namespace Chill.ExampleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Web.Http;


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
    }



    
}
