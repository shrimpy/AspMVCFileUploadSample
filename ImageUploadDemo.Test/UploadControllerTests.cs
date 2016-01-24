using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageUploadDemo.controllers;
using ImageUploadDemo.ImageProcessProvider;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ImageUploadDemo.Test
{
    public class UploadControllerTests
    {
        [Fact]
        public async Task BasicTests()
        {
            var imageProviderMock = new Mock<IImageProvider>();
            imageProviderMock
                .Setup(i => i.Resize(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.FromResult(new ResizeResult
                {
                    UriActualSize = "http://actual",
                    UriResizedSize = "http://resize"
                }));

            var formCollectionMock = new Mock<IFormCollection>();
            var formFileCollectionMock = new Mock<IFormFileCollection>();
            var formFileMock = new Mock<IFormFile>();
            formCollectionMock.Setup(f => f.Files).Returns(formFileCollectionMock.Object);
            formFileCollectionMock.Setup(f => f.Count).Returns(1);
            formFileCollectionMock.Setup(f => f.GetEnumerator()).Returns((new List<IFormFile>() { formFileMock.Object }).GetEnumerator());
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            formFileMock.Setup(f => f.ContentDisposition).Returns("form-data; name=\"photo\"; filename=\"kneeph.jpg\"");
            formFileMock.Setup(f => f.ContentType).Returns("image/jpeg");

            var httpRequestMock = new DefaultHttpRequest(new DefaultHttpContext(), Mock.Of<IFeatureCollection>());
            httpRequestMock.Form = formCollectionMock.Object;
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(h => h.Request).Returns(httpRequestMock);

            UploadController controller = new UploadController(imageProviderMock.Object, Mock.Of<ILoggerFactory>());
            controller.ActionContext = new Microsoft.AspNet.Mvc.ActionContext()
            {
                HttpContext = httpContext.Object
            };

            ObjectResult result = await controller.Post() as ObjectResult;

            Assert.NotNull(result);
            string valStr = result.Value.ToString();
            // { originUrl = "http://actual", url = "http://resize", error = null }

            Assert.True(valStr.Contains("originUrl = http://actual"));
            Assert.True(valStr.Contains("url = http://resize"));
        }

        [Fact]
        public async Task ErrorTests()
        {
            var imageProviderMock = new Mock<IImageProvider>();
            imageProviderMock
                .Setup(i => i.Resize(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(() =>
                {
                    throw new Exception("Foo");
                });

            var formCollectionMock = new Mock<IFormCollection>();
            var formFileCollectionMock = new Mock<IFormFileCollection>();
            var formFileMock = new Mock<IFormFile>();
            formCollectionMock.Setup(f => f.Files).Returns(formFileCollectionMock.Object);
            formFileCollectionMock.Setup(f => f.Count).Returns(1);
            formFileCollectionMock.Setup(f => f.GetEnumerator()).Returns((new List<IFormFile>() { formFileMock.Object }).GetEnumerator());
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            formFileMock.Setup(f => f.ContentDisposition).Returns("form-data; name=\"photo\"; filename=\"kneeph.jpg\"");
            formFileMock.Setup(f => f.ContentType).Returns("image/jpeg");

            var httpRequestMock = new DefaultHttpRequest(new DefaultHttpContext(), Mock.Of<IFeatureCollection>());
            httpRequestMock.Form = formCollectionMock.Object;
            var httpContext = new Mock<HttpContext>(); // new DefaultHttpContext();
            httpContext.Setup(h => h.Request).Returns(httpRequestMock);

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(Mock.Of<ILogger>());
            UploadController controller = new UploadController(imageProviderMock.Object, loggerFactoryMock.Object);
            controller.ActionContext = new Microsoft.AspNet.Mvc.ActionContext()
            {
                HttpContext = httpContext.Object
            };

            Exception exception = null;

            try
            {
                await controller.Post();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.Equal("Foo", exception.Message);
        }
    }
}
