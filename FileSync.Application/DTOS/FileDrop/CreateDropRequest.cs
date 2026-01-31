using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application.DTOS.FileDrop
{
    public class CreateDropRequest
    {
        public List<string>? RecipientEmails { get; set; } = new List<string>();
        public string? Passcode { get; set; }
        public string? Message { get; set; }
        public int ExpiresInDays { get; set; } = 3;
    }
}
