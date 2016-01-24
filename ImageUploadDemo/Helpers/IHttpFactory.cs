using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageUploadDemo.Helpers
{
    public interface IHttpFactory
    {
        IHttpClientWrapper CreateHttpClient();
    }
}
