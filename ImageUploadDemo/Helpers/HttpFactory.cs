namespace ImageUploadDemo.Helpers
{
    public class HttpFactory : IHttpFactory
    {
        public IHttpClientWrapper CreateHttpClient()
        {
            return new HttpClientWrapper();
        }
    }
}
