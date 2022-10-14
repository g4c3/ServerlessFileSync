using System;

namespace FileSync.DTOs;
internal class DoDRequest : FileRequest
{    
    public Guid BlobId { get; set; } = Guid.NewGuid();
}
