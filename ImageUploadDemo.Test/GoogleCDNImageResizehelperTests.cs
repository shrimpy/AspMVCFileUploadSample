using System;
using System.Globalization;
using System.Threading.Tasks;
using ImageUploadDemo.Helpers;
using ImageUploadDemo.ImageProcessProvider;
using Moq;
using Xunit;

namespace ImageUploadDemo.Test
{
    public class GoogleCDNImageResizehelperTests
    {
        [Fact]
        public async Task BasicTests()
        {
            string rawImageUrl = "http://foo.com/bar.jpg";
            int maxHeight = 700;
            int maxWide = 500;

            string uriMaxHeight = string.Format(
                CultureInfo.InvariantCulture,
                GoogleCDNImageResizehelper.ResizeApiTemplate,
                Uri.EscapeDataString(rawImageUrl),
                "resize_h=" + maxHeight);

            string uriMaxWide = string.Format(
                CultureInfo.InvariantCulture,
                GoogleCDNImageResizehelper.ResizeApiTemplate,
                Uri.EscapeDataString(rawImageUrl),
                "resize_w=" + maxWide);

            var httpFactoryMock = new Mock<IHttpFactory>();
            var httpClientMock = new Mock<IHttpClientWrapper>();
            httpFactoryMock.Setup(h => h.CreateHttpClient()).Returns(httpClientMock.Object);
            httpClientMock.Setup(h => h.GetStringAsync(uriMaxHeight)).Returns(Task.FromResult("long string"));
            httpClientMock.Setup(h => h.GetStringAsync(uriMaxWide)).Returns(Task.FromResult("string"));

            var helper = new GoogleCDNImageResizehelper(httpFactoryMock.Object);
            string result = await helper.Resize(rawImageUrl, maxHeight, maxWide);
            Assert.Equal(uriMaxWide, result);
        }
    }
}
