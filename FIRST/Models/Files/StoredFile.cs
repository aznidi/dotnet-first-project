namespace FIRST.Models.Files;

public class StoredFile
{
    public Guid Id { get; set; }

    public string OriginalName { get; set; } = default!;
    public string StoredName { get; set; } = default!;   
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size { get; set; }

    public string RelativePath { get; set; } = default!;  
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? UploadedByUserId { get; set; }
}
