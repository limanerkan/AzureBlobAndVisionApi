using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBlobAndVisionApi.Classes
{
    public class ComputerVisionManager
    {
        private VisionServiceClient _vision;
        private VisualFeature[] _features;
        private readonly string key = "";

        public ComputerVisionManager()
        {
            _vision=new VisionServiceClient(key);
            _features= new VisualFeature[] { VisualFeature.Description, VisualFeature.Categories };
        }
        public async Task<bool> ImageTag(CloudBlockBlob blockBlob)
        {
            var result = await _vision.AnalyzeImageAsync(blockBlob.Uri.ToString(), _features);

            blockBlob.Metadata.Add("Caption", result.Description.Captions[0].Text);
            blockBlob.Metadata.Add("Categories", result.Categories.FirstOrDefault().Name.ToString());

            for (int i = 0; i < result.Description.Tags.Length; i++)
            {
                string key = String.Format("Tag{0}", i);
                blockBlob.Metadata.Add(key, result.Description.Tags[i]);
            }

            await blockBlob.SetMetadataAsync();



            return true;
        }
    }
}
