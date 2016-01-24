using System;
using System.IO;
using System.Threading.Tasks;
using ImageUploadDemo.models;
using Microsoft.Extensions.OptionsModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageUploadDemo.ImageProcessProvider
{
    public class AzureBlobProvider : IImageProvider
    {
        private IOptions<AppSettings> appSettings;
        private IResizer resizer;

        public AzureBlobProvider(IOptions<AppSettings> appSettings, IResizer resizer)
        {
            this.appSettings = appSettings;
            this.resizer = resizer;
        }

        public object Acount { get; private set; }

        public async Task<ResizeResult> Resize(string fileName, Stream stream)
        {
            StorageCredentials cred = new StorageCredentials(this.appSettings.Value.StorageAccount, this.appSettings.Value.StorageKey);
            CloudStorageAccount account = new CloudStorageAccount(cred, false);
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("images");
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Blob,
            });
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(stream);

            return new ResizeResult()
            {
                UriActualSize = blob.Uri.AbsoluteUri,
                UriResizedSize = await this.resizer.Resize(blob.Uri.AbsoluteUri, Constants.MaxHeight, Constants.MaxWide)
            };
        }
    }
}
