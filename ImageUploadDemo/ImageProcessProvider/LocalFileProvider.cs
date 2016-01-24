using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using ImageUploadDemo.Helpers;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;

namespace ImageUploadDemo.ImageProcessProvider
{
    /// <summary>
    /// Can only run on actual deployment since we rely on real URl to use Google CDN to resize image
    /// </summary>
    public class LocalFileProvider : IImageProvider
    {
        private IHostingEnvironment env;
        private IHttpContextAccessor httpContextAccessor;
        private IResizer resizer;
        private IFileSystemHelper fileSystemHelper;

        public LocalFileProvider(
            IHostingEnvironment env, 
            IHttpContextAccessor httpContextAccessor, 
            IResizer resizer,
            IFileSystemHelper fileSystemHelper)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
            this.resizer = resizer;
            this.fileSystemHelper = fileSystemHelper;
        }

        public async Task<ResizeResult> Resize(string fileName, Stream stream)
        {
            string wwwroot = this.env.WebRootPath;

            string folderpath = Path.Combine(wwwroot, "dl");
            if (!this.fileSystemHelper.isDirectoryExists(folderpath))
            {
                this.fileSystemHelper.CreateDirectory(folderpath);
            }

            string filePath = Path.Combine(folderpath, fileName);
            using (var fs = this.fileSystemHelper.CreateFile(filePath))
            {
                await stream.CopyToAsync(fs);
            }

            string rawUri = string.Format(CultureInfo.InvariantCulture,
                "{0}://{1}/dl/{2}",
                this.httpContextAccessor.HttpContext.Request.Scheme,
                this.httpContextAccessor.HttpContext.Request.Host,
                fileName);
            return new ResizeResult()
            {
                UriActualSize = rawUri,
                UriResizedSize = await this.resizer.Resize(rawUri, Constants.MaxHeight, Constants.MaxWide)
            };
        }
    }
}
