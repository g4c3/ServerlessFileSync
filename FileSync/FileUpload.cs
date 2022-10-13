using System.Net;
using System.Threading.Tasks;
using FileSync.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace FileSync;

public class FileUpload
{
    public FileUpload(){ }

    [FunctionName("UploadFiles")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        FileUploadRequestObject requestObect = new()
        {
            ContainerName = req.Form["ContainerName"],
            Files = req.Form.Files,
            Path = req.Form["Path"]
        };

        var blobStorageConnector = new BlobStorageConnector(requestObect.ContainerName);
        var response = await blobStorageConnector.SaveFilesAsync(requestObect.Files, requestObect.Path);

        return new OkObjectResult(response);
    }

    private sealed class FileUploadRequestObject
    {
        public string ContainerName { get; set; }
        public IFormFileCollection Files { get; set; }
        public string Path { get; set; }
    }
    
}

