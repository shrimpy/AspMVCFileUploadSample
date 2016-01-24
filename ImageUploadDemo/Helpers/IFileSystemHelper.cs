using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageUploadDemo.Helpers
{
    /// <summary>
    /// Create our own file system abstraction, since System.IO.Abstractions is not support DNX yet
    /// </summary>
    public interface IFileSystemHelper
    {
        bool isDirectoryExists(string path);

        DirectoryInfo CreateDirectory(string path);

        FileStream CreateFile(string path);
    }
}
