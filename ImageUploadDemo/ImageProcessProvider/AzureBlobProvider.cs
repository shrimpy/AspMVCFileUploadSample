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
    public class AzureBlobProvider : IProvider
    {
        private const string CloudName = "DEFAULT_CLOUD";
        IOptions<AppSettings> appSettings;

        public AzureBlobProvider(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings;
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
                UriResizedSize = blob.Uri.AbsoluteUri
            };
        }
    }
}
