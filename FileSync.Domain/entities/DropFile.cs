namespace FileSync.Domain.entities
{
    public class DropFile
    {
        public Guid Id { get; set; }
        public Guid DropId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }

        public FileDrop Drop { get; set; } = null!;
    }
}