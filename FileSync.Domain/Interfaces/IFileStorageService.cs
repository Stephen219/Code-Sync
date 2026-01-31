using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Domain.Interfaces
{
public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder);
        Task<Stream?> GetFileAsync(string storagePath);
        Task DeleteFileAsync(string storagePath);
        Task DeleteFolderAsync(string folder);
    }
}
