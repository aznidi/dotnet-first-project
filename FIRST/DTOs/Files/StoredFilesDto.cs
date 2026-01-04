namespace FIRST.DTOs.Files
{
    public class StoredFilesDto
    {
        public Guid Id { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }

        public string DownloadUrl { get; set; } = string.Empty;
    }
}
