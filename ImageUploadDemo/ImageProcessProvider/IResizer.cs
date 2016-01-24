using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageUploadDemo.ImageProcessProvider
{
    public interface IResizer
    {
        Task<string> Resize(string rawImageUrl, int maxHeight, int maxWide);
    }
}
