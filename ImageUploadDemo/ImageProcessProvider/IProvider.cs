using System.IO;
using System.Threading.Tasks;

namespace ImageUploadDemo.ImageProcessProvider
{
    public interface IProvider
    {
        /// <param name="fileName">File Name</param>
        /// <param name="stream">File content</param>
        /// <returns>Resize result</returns>
        Task<ResizeResult> Resize(string fileName, Stream stream);
    }

    public class ResizeResult
    {
        public string UriActualSize { get; set; }
        public string UriResizedSize { get; set; }
    }
}
