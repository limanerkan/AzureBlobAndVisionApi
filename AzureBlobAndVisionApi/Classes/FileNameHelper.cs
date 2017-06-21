using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobAndVisionApi.Classes
{
    public class FileNameHelper
    {
        public static string CreateFileName()
        {
            return Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
        }
    }
}
