using System.IO;

namespace ImageUploadDemo.Helpers
{
    public class FileSystemHelper : IFileSystemHelper
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public FileStream CreateFile(string path)
        {
            return File.Create(path);
        }

        public bool isDirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
