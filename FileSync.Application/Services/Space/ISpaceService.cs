using FileSync.Application.DTOS.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application.Services.Space
{
    public interface ISpaceService
    {
        Task<CreateSpaceResponse> CreateSpaceAsync();
        Task<SpaceDto?> GetSpaceByCodeAsync(string code);
        Task<bool> UpdateContentAsync(string code, string content);
        Task<SpaceFileDto?> AddFileAsync(string code, Stream fileStream, string fileName, string contentType);
        Task<Stream?> GetFileAsync(string code, Guid fileId);
        Task<bool> DeleteSpaceAsync(string code);
    }
}
