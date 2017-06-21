using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AzureBlobAndVisionApi.Classes
{
    public class FileOperation : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() == "apiımageuploadpost")
            {
                operation.Parameters.Clear();//Clearing parameters
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "File",
                    In = "formData",
                    Description = "Uplaod Image",
                    Required = true,
                    Type = "file"
                });
                operation.Consumes.Add("application/form-data");
            }
        }
    }
}
