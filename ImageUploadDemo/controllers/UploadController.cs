using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageUploadDemo.ImageProcessProvider;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace ImageUploadDemo.controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private IImageProvider imageProvider;
        private ILoggerFactory loggerFactory;
        public UploadController(IImageProvider imageProvider, ILoggerFactory loggerFactory)
        {
            this.imageProvider = imageProvider;
            this.loggerFactory = loggerFactory;
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

            IFormFile file = Request.Form.Files.First();
            if (!(string.Equals("image/jpeg", file.ContentType, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("image/jpg", file.ContentType, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("image/png", file.ContentType, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Invalid content type. Only support JPG and PNG images.");
            }

            string actualFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            Console.WriteLine(file.ContentType);
            FileInfo fi = new FileInfo(actualFileName);
            string fileName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Guid.NewGuid().ToString("N").Substring(0, 8), fi.Extension);

            ResizeResult result = null;

            try
            {
                using (Stream stream = file.OpenReadStream())
                {
                    result = await this.imageProvider.Resize(fileName, stream);
                }
            }
            catch (Exception ex)
            {
                this.loggerFactory.CreateLogger("Default").LogError("", ex);
                throw;
            }

            return new ObjectResult(new
            {
                originUrl = result.UriActualSize,
                url = result.UriResizedSize
            });
        }
    }
}
