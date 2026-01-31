using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application.DTOS.Space
{
    public class SpaceDto
    {
        public string Code { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public List<SpaceFileDto> Files { get; set; } = new();
    }

    public class SpaceFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
    }
}
