using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace AzureBlobAndVisionApi.Classes
{
    public class BlobManager
    {
        #region PrivateMember
        private string _accountName = "";
        private string _containerName = "";
        private string _accountKey = "";
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        private CloudBlockBlob _blockBlob;
        private Stream _stream;
        private List<Images> _blobs;
        private BlobContinuationToken _token = null;
        private ComputerVisionManager _manager;
        #endregion

        #region PublicMember

        //public string BlobName { get => _blobName; set => _blobName = value; }
        //public string ContainerName { get => _containerName; set => _containerName = value; }
        //public string AccountKey { get => _accountKey; set => _accountKey = value; }

        #endregion

        #region Constructor

        public BlobManager()
        {
            _storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    _accountName,
                    _accountKey), true);
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _container = _blobClient.GetContainerReference(_containerName);
            _manager = new ComputerVisionManager();
        }

        #endregion


        public async Task<string> UploadImageAsBlob(IFormFile file)
        {
            if (file != null && file.ContentType.StartsWith("image"))
            {
                _stream = file.OpenReadStream();
                _blockBlob = _container.GetBlockBlobReference(FileNameHelper.CreateFileName());
                await _blockBlob.UploadFromStreamAsync(_stream);
                var result = _manager.ImageTag(_blockBlob);
                if (await result)
                    return _blockBlob.Uri.ToString();
            }
            return null;
        }

        public async Task<List<Images>> GetImagesAsync(string term)
        {
            _blobs = new List<Images>();
            _container = _blobClient.GetContainerReference("profileimages");

            do
            {
                BlobResultSegment resultSegment = await _container.ListBlobsSegmentedAsync(_token);
                _token = resultSegment.ContinuationToken;

                foreach (IListBlobItem item in resultSegment.Results)
                {
                    var blob = item as CloudBlockBlob;
                    if (blob != null)
                    {
                        await blob.FetchAttributesAsync(); // Get blob metadata
                        foreach (var items in blob.Metadata)
                        {
                            if (items.Key.StartsWith("Tag") && items.Value.Equals(term, StringComparison.CurrentCultureIgnoreCase))
                            {
                                var caption = blob.Metadata.ContainsKey("Caption") ? blob.Metadata["Caption"] : blob.Name;
                                var categories = blob.Metadata.ContainsKey("Categories") ? blob.Metadata["Categories"] : blob.Name;
                                _blobs.Add(new Images()
                                {
                                    ImageUri = blob.Uri.ToString(),
                                    Categories = categories,
                                    Caption = caption
                                });
                            }

                        }
                    }
                }
            } while (_token != null);

            return _blobs;
        }
    }
}
