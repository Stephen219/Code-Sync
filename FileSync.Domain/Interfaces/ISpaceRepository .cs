using FileSync.Domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Domain.Interfaces
{
     public interface ISpaceRepository
    {


        Task<Space?> GetByIdAsync(Guid id);
        Task<Space?> GetByCodeAsync(string code);
        Task AddAsync(Space space);
        Task UpdateAsync(Space space);
        Task DeleteAsync(Guid id);


        Task AddFileAsync(SpaceFile file);  
    }
}
