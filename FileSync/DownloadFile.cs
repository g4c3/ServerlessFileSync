using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
        FileDownloadRequestObject requestObect = JsonConvert.DeserializeObject<FileDownloadRequestObject>(stream);

        var blobStorageConnector = new BlobStorageConnector(requestObect.ContainerName);
        var response = await blobStorageConnector.GetFileContentAsync(requestObect.Path, requestObect.BlobId);

        Stream blobStream = response.Content.ToStream();
        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);

        message.Content = new StreamContent(blobStream);
        message.Content.Headers.ContentLength = response.Details.ContentLength;
        message.StatusCode = HttpStatusCode.OK;
        message.Content.Headers.ContentType = new MediaTypeHeaderValue(response.Details.ContentType);
        message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = $"CopyOf_{response.Details.CopyId}",
            Size = response.Details.ContentLength
        };

        return message;
        
    }

    private sealed class FileDownloadRequestObject
    {
        public string? ContainerName { get; set; }
        public string? Path { get; set; }
        public Guid BlobId { get; set; } = Guid.NewGuid();
    }
}

