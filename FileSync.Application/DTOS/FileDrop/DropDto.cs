using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application.DTOS.FileDrop
{
    public class DropDto
    {
        public string Code { get; set; } = string.Empty;
        public string? Message { get; set; }
        public bool HasPasscode { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int DownloadCount { get; set; }
        public List<DropFileDto> Files { get; set; } = new();
    }

    public class DropFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}
