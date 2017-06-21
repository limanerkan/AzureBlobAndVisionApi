using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
namespace AzureBlobAndVisionApi.Classes
{
    public class Upload
    {

        public static async Task<string> UploadFileAsBlob(Stream stream, string filename)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=votedbblob;AccountKey=d9u7pWRx1zKj6lrm+TTI31wD+xcUekWOnVfZBYD79ytUAuDfYGs8o4uFHAhuQJiyNp9vrQnsqYJnerZHoiXcIw==;EndpointSuffix=core.windows.net");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("profileimages");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);

            await blockBlob.UploadFromStreamAsync(stream);

            VisionServiceClient vision = new VisionServiceClient("04fc2ad11ddb48968378d40f73428890");
            VisualFeature[] features = new VisualFeature[] { VisualFeature.Description,VisualFeature.Categories};
            var result = await vision.AnalyzeImageAsync(blockBlob.Uri.ToString(), features);
            
            blockBlob.Metadata.Add("Caption", result.Description.Captions[0].Text);
            blockBlob.Metadata.Add("Categories",result.Categories.FirstOrDefault().Name.ToString());

            for (int i = 0; i < result.Description.Tags.Length; i++)
            {
                string key = String.Format("Tag{0}", i);
                blockBlob.Metadata.Add(key, result.Description.Tags[i]);
            }

            await blockBlob.SetMetadataAsync();
            stream.Dispose();
            return blockBlob?.Uri.ToString();
        }

        private bool HasMatchingMetadata(CloudBlockBlob blob, string term)
        {
            foreach (var item in blob.Metadata)
            {
                if (item.Key.StartsWith("Tag") && item.Value.Equals(term, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }
        public static async Task ReturnaAsync(string term)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    "votedbblob",
                    "d9u7pWRx1zKj6lrm+TTI31wD+xcUekWOnVfZBYD79ytUAuDfYGs8o4uFHAhuQJiyNp9vrQnsqYJnerZHoiXcIw=="), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            List<Images> blobs = new List<Images>();
            // Get a reference to a container named "mycontainer."
            CloudBlobContainer container = blobClient.GetContainerReference("profileimages");

            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;
                
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
                                var categories= blob.Metadata.ContainsKey("Categories") ? blob.Metadata["Categories"] : blob.Name;
                                blobs.Add(new Images()
                                {
                                    ImageUri = blob.Uri.ToString(),
                                    Categories = categories,
                                    Caption = caption
                                });
                            }

                        }
                    }
                }
            } while (token != null);

            for (int i = 0; i < blobs.Count; i++)
            {
                Console.WriteLine("{0},{1},{2}", blobs[i].Caption, blobs[i].ImageUri,blobs[i].Categories);
            }
        }
    }
}
