using FileSync.Domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Domain.Interfaces
{
    public interface IFileDropRepository
    {
        Task<FileDrop?> GetByIdAsync(Guid id);
        Task<FileDrop?> GetByCodeAsync(string code);
        Task AddAsync(FileDrop drop);
        Task UpdateAsync(FileDrop drop);
        Task DeleteAsync(Guid id);
        Task<List<FileDrop>> GetExpiredDropsAsync();

        Task AddFileAsync(DropFile file);
    }
}
