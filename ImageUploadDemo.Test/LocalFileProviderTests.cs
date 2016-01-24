using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageUploadDemo.Helpers;
using ImageUploadDemo.ImageProcessProvider;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Internal;
using Moq;
using Xunit;

namespace ImageUploadDemo.Test
{
    public class LocalFileProviderTests
    {
        [Fact]
        public async Task BasicTests()
        {
            string tmpFile = Path.GetTempFileName();
            FileStream tmpFileStream = new FileStream(tmpFile, FileMode.Create);
            MemoryStream tmpStream = new MemoryStream();
            try
            {
                var fileSystemMock = new Mock<IFileSystemHelper>();
                fileSystemMock.Setup(f => f.isDirectoryExists(It.IsAny<string>())).Returns(true);
                fileSystemMock.Setup(f => f.CreateFile(It.IsAny<string>())).Returns(tmpFileStream);

                var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
                hostingEnvironmentMock.Setup(h => h.WebRootPath).Returns("x:\\tmp");

                var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
                var httpContextMock = new Mock<HttpContext>();
                httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContextMock.Object);
                httpContextMock.Setup(h => h.Request.Host).Returns(new HostString("foo"));
                httpContextMock.Setup(h => h.Request.Scheme).Returns("https");

                var resizerMock = new Mock<IResizer>();
                resizerMock.Setup(r => r.Resize(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult("http://resized"));

                var localFileProvider = new LocalFileProvider(
                    hostingEnvironmentMock.Object,
                    httpContextAccessorMock.Object,
                    resizerMock.Object,
                    fileSystemMock.Object);

                ResizeResult result = await localFileProvider.Resize("foo.jpg", tmpStream);
                Assert.Equal("https://foo/dl/foo.jpg", result.UriActualSize);
                Assert.Equal("http://resized", result.UriResizedSize);
            }
            finally
            {
                tmpFileStream.Dispose();
                tmpStream.Dispose();
            }
        }
    }
}
