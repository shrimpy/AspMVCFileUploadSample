using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using ImageUploadDemo.Helpers;

namespace ImageUploadDemo.ImageProcessProvider
{
    /// <summary>
    /// ASP.NET 5 (ASP.NET Core 1.0) doesn`t support System.Draw, doesn`t support any image manipulation.
    /// Will need to invoke image service to perform resizing.
    /// 
    /// Google CDN seems provide free image resizing. But seems like there is a size a limit at around 5MB
    /// </summary>
    public class GoogleCDNImageResizehelper : IResizer
    {
        public const string ResizeApiTemplate = "https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?url={0}&container=focus&{1}&refresh=31536000";
        public const string NoResizeApiTemplate = "https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?url={0}&container=focus&refresh=31536000";
        private IHttpFactory httpFactory;

        public GoogleCDNImageResizehelper(IHttpFactory httpFactory)
        {
            this.httpFactory = httpFactory;
        }

        public async Task<string> Resize(string rawImageUrl, int maxHeight, int maxWide)
        {
            string uriMaxHeight = string.Format(
                CultureInfo.InvariantCulture,
                ResizeApiTemplate,
                Uri.EscapeDataString(rawImageUrl),
                "resize_h=" + maxHeight);

            string uriMaxWide = string.Format(
                CultureInfo.InvariantCulture,
                ResizeApiTemplate,
                Uri.EscapeDataString(rawImageUrl),
                "resize_w=" + maxWide);

            string uriNoResize = string.Format(
                CultureInfo.InvariantCulture,
                NoResizeApiTemplate,
                Uri.EscapeDataString(rawImageUrl));

            using (IHttpClientWrapper client = this.httpFactory.CreateHttpClient())
            {
                var contentHTask = client.GetStringAsync(uriMaxHeight);
                var contentWTask = client.GetStringAsync(uriMaxWide);
                var noResizeTask = client.GetStringAsync(uriNoResize);
                await Task.WhenAll(noResizeTask, contentHTask, contentWTask);

                int minLen = Math.Min(noResizeTask.Result.Length, Math.Min(contentHTask.Result.Length, contentWTask.Result.Length));

                if (minLen == contentHTask.Result.Length)
                    return uriMaxHeight;
                else if (minLen == contentWTask.Result.Length)
                    return uriMaxWide;

                return rawImageUrl;
            }
        }
    }
}
