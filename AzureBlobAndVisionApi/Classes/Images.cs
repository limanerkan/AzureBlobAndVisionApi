using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobAndVisionApi.Classes
{
    public class Images
    {
        public string ImageUri { get; set; }
        public string Caption { get; set; }
        public string Categories { get; set; }
    }
}
