using System;

namespace FileSync.DTOs;
internal class DODRequest: FileRequest
{    
    public Guid BlobId { get; set; } = Guid.NewGuid();
}
