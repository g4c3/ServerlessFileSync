using System;
using System.Text.Json.Serialization;

namespace FileSync.Entities;
internal class FileModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StorageFullPath { get; set; }
    public string StorageName { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastModified { get; set; }
    public string FileName { get; set; }
    public string ContentHash { get; set; }
    public long ContentLength { get; set; }
    public string ContentType { get; set; }

    //public IDictionary<string, string> CustomFields { get; set; }
}
