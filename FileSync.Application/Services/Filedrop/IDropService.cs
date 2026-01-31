using FileSync.Application.DTOS.FileDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application.Services.Filedrop
{
    public interface IDropService
    {
        Task<CreateDropResponse> CreateDropAsync(CreateDropRequest request);
        Task<DropFileDto?> AddFileAsync(Guid dropId, Stream fileStream, string fileName, string contentType);
        Task<bool> FinalizeDropAsync(Guid dropId);
        Task<DropDto?> GetDropByCodeAsync(string code);
        Task<bool> VerifyPasscodeAsync(string code, string passcode);
        Task<Stream?> GetFileAsync(string code, Guid fileId);

    }
}
