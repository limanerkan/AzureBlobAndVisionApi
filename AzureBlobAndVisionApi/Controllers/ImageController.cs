using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureBlobAndVisionApi.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobAndVisionApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Image")]
    [Consumes("multipart/form-data")]
    public class ImageController : Controller
    {
        // POST: api/Image
     
        [HttpPost]
        [Route("upload")]
        public async Task<string> PostAsync(IFormFile file)
        {
            
            BlobManager manager =new BlobManager();
            var imageUrl = manager.UploadImageAsBlob(file);
            return await imageUrl;
        }


        [HttpGet]
        public async Task<List<Images>> Get(string req)
        {
            BlobManager manager = new BlobManager();
            List<Images> img = await manager.GetImagesAsync(req);
            return img;
        }
    }
}
