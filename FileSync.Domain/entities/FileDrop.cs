using FileSync.Domain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Domain.entities
{
    public class FileDrop

    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        //public string? RecipientEmail { get; set; }
        public List<string>? RecipientEmails { get; set; } = new List<string>();
        public string? PasscodeHash { get; set; }
        public string? Message { get; set; }
        public DropStatus Status { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public List<DropFile> Files { get; set; } = new();
    }
}
