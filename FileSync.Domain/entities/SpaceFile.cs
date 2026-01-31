namespace FileSync.Domain.entities
{
    public class SpaceFile
    {
       

        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public long FileSize { get; set; }
        public required string FileType { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid SpaceId { get; set; }
        public required string StoragePath { get; set; }


        public Space Space { get; set; }

    }



}