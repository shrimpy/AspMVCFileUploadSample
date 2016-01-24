using System;
using System.Threading.Tasks;

namespace ImageUploadDemo.Helpers
{
    public interface IHttpClientWrapper : IDisposable
    {
        Task<string> GetStringAsync(string uri);
    }
}
