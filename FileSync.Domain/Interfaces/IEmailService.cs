using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendDropNotificationAsync(List<string> recipients, string dropCode, string? message);
    }
}
