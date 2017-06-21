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
            if (file != null && file.ContentType.StartsWith("image"))
            {
                var stream = file.OpenReadStream();
                //var name = Path.GetFileName(file.FileName);
                var name = Convert.ToString(Guid.NewGuid()) + ".jpg";
                var uploadfile = await Upload.UploadFileAsBlob(stream, name);

                return uploadfile;
            }
            return null;
        }

       

    }
}
