using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageUploadDemo.models;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Net.Http.Headers;

namespace ImageUploadDemo.controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        IOptions<AppSettings> appSettings;

        public UploadController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            if (Request.Form.Files.Count != 1)
                throw new InvalidOperationException("Invalid upload!");

            string wwwroot = Environment.GetEnvironmentVariable("SITE_WWWROOT");
            if (string.IsNullOrWhiteSpace(wwwroot))
                wwwroot = this.appSettings.Value.wwwroot;

            IFormFile file = Request.Form.Files.First();
            string actualFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            FileInfo fi = new FileInfo(actualFileName);
            string fileName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Guid.NewGuid().ToString("N").Substring(0, 8), fi.Extension);
            string filePath = Path.Combine(wwwroot, "dl", fileName);
            await file.SaveAsAsync(filePath);

            return new ObjectResult(new
            {
                url = string.Format(CultureInfo.InvariantCulture, "{0}://{1}/dl/{2}", Request.Scheme, Request.Host.Value, fileName)
            });
        }
    }
}
