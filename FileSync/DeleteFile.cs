using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileSync.DTOs;
using FileSync.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FileSync;

public class DeleteFile
{
    public DeleteFile(){ }

    [FunctionName("DeleteFile")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req)
    {
        var stream = await new StreamReader(req.Body, Encoding.UTF8).ReadToEndAsync();
        DoDRequest requestObect = JsonConvert.DeserializeObject<DoDRequest>(stream)!;

        var blobStorageConnector = new BlobStorageConnector(requestObect.ContainerName!);
        var response = await blobStorageConnector.DeleteFileAsync(requestObect.Path!, requestObect.BlobId);

        return new OkObjectResult(response);
    }
}

