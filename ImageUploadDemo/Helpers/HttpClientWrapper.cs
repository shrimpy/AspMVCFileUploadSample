using System.Net.Http;
using System.Threading.Tasks;

namespace ImageUploadDemo.Helpers
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private HttpClient client;

        public HttpClientWrapper()
        {
            this.client = new HttpClient();
        }

        public void Dispose()
        {
            this.client.Dispose();
        }

        public Task<string> GetStringAsync(string uri)
        {
            return this.client.GetStringAsync(uri);
        }
    }
}
