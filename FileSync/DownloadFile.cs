using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FileSync.DTOs;
using FileSync.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FileSync;

public class DownloadFile
{
    public DownloadFile() { }

    [FunctionName("DownloadFile")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
    {
        var stream = await new StreamReader(req.Body, Encoding.UTF8).ReadToEndAsync();
        DODRequest requestObect = JsonConvert.DeserializeObject<DODRequest>(stream)!;

        var blobStorageConnector = new BlobStorageConnector(requestObect!.ContainerName!);
        var content = await blobStorageConnector.GetFileContentAsync(requestObect.Path!, requestObect.BlobId);

        HttpResponseMessage message = new();

        message.Content = new StreamContent(content.Stream!);
        message.Content.Headers.ContentLength = content.ContentLength;
        message.StatusCode = HttpStatusCode.OK;
        message.Content.Headers.ContentType = new MediaTypeHeaderValue(content.ContentType!);
        message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = $"CopyOf_{content.FileName}",
            Size = content.ContentLength
        };

        return message;
    }
}

