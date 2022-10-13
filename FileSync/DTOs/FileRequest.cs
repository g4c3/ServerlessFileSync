namespace FileSync.DTOs;
internal abstract class FileRequest
{    
    public string? ContainerName { get; set; }
    public string? Path { get; set; }

}
