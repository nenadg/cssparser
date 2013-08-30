using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

// System.IO is needed for file operations
using System.IO;
using CssParser.Models;

namespace CssParser.Controllers
{

    public class CssParseController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage Post(IEnumerable<CssProperty> tochange)
        {
            var pathOriginal = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "static/css/demo.css");
            FileInfo templejtFajl = new FileInfo(pathOriginal);
            
            new Parser().ParseCssFile(new FileInfo(pathOriginal).OpenText(), tochange);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            return result;
        }

        [HttpGet]
        public HttpResponseMessage Download()
        {
            var pathChanged = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "static/css/demo-new.css");
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(pathChanged, FileMode.Open);

            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "demo-new.css";
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/css");
            result.Content.Headers.ContentLength = stream.Length;

            return result;
        }


    }
}
