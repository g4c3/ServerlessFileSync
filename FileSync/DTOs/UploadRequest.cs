using Microsoft.AspNetCore.Http;

namespace FileSync.DTOs;
internal class UploadRequest: FileRequest
{
    public IFormFileCollection? Files { get; set; }
}
