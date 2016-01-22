using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageUploadDemo.ImageProcessProvider
{
    public class LocalFileProvider : IProvider
    {
        public Task<ResizeResult> Resize(string fileName, Stream stream)
        {
            throw new NotSupportedException("No image process library support for asp.net core 1.0 yet.");
        }
    }
}
