using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using ModelAccessLayer;
using BussinessAccessLayer;
using System.Web.Http;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestUserApp.Controllers
{
    [EnableCors(origins: "*", methods: "*", headers: "*")]
    public class AccountController : ApiController
    {
        private readonly BLAccounts bLAccounts;
        public static string id = null;
        public AccountController()
        {
            bLAccounts = new BLAccounts();
        }


        [Route("Login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] Login loginRequest)
        {
            var results= bLAccounts.UserLogin(loginRequest, out MainResponse result);
            switch(result.Status)
            {
                case "Success":
                    break;
                case "BadRequest":
                    break;
            }
            return Ok(results);

        }

        [Route("Register")]
        [HttpPost]
        public IHttpActionResult Register([FromBody] Register registerRequest)
        {
            var results = bLAccounts.UserRegister(registerRequest);
            return Ok(results);

        }

        [Route("UploadFile")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFile()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent()) // Fails here 
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    var files = JsonConvert.DeserializeObject(file.Headers.ContentDisposition.FileName);
                    var fileName= "Server file path: " + file.LocalFileName;
                }
                return Ok(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Ok(HttpStatusCode.InternalServerError);
            }

        }
    }
}