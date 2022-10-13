using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileSync.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSync.Services;
internal class BlobStorageConnector
{
    private readonly string _blobConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    private readonly BlobContainerClient _blobContainerClient;

    internal BlobStorageConnector(string containerName)
    {
        BlobServiceClient _blobService = new(_blobConnectionString);
        _blobContainerClient = _blobService.GetBlobContainerClient(containerName);
        _blobContainerClient.CreateIfNotExists();
    }

    internal async Task<List<FileModel>> SaveFilesAsync(IFormFileCollection files, string directoryPath, System.Threading.CancellationToken ct = default)
    {
        List<FileModel> result = new();
        foreach (var file in files)
        {
            var blobId = Guid.NewGuid();
            _ = new MemoryStream();
            Stream myBlob = file.OpenReadStream();
            var blob = _blobContainerClient.GetBlobClient($"{directoryPath}{'/'}{blobId}");
            var header = new BlobHttpHeaders { ContentType = file.ContentType };

            await blob.UploadAsync(myBlob, header, cancellationToken: ct);

            var properties = (await blob.GetPropertiesAsync(cancellationToken: ct)).Value;
            
            var blobModel = new FileModel
            {
                Id = blobId,
                StorageFullPath = blob.Uri.AbsoluteUri,
                StorageName = blob.Name,
                CreatedOn = properties.CreatedOn.DateTime,
                LastModified = properties.LastModified.DateTime,
                FileName = file.FileName,
                ContentHash = BitConverter.ToString(properties.ContentHash).Replace("-", ""),
                ContentLength = properties.ContentLength,
                ContentType = properties.ContentType
            };
            result.Add(blobModel);
        }

        return result;
    }

    internal async Task<BlobDownloadInfo> GetBlobInfoAsync(string directoryPath, System.Threading.CancellationToken ct = default)
    {
        var blobClient = _blobContainerClient.GetBlobClient(directoryPath);
        var blobInfo = await blobClient.DownloadAsync(ct);

        return blobInfo;
    }

    internal async Task<bool> DeleteFileAsync(string directoryPath, Guid blobId) => 
        (await _blobContainerClient.DeleteBlobIfExistsAsync($"{directoryPath}{'/'}{blobId}")).Value;

    internal async Task<BlobDownloadResult> GetFileContentAsync(string directoryPath, Guid blobId, System.Threading.CancellationToken ct = default)
    {
        var blob = _blobContainerClient.GetBlobClient($"{directoryPath}{'/'}{blobId}");
        await blob.DownloadAsync(ct);
        var blobInfo = await blob.DownloadContentAsync(ct);

        return blobInfo;
    }
}
