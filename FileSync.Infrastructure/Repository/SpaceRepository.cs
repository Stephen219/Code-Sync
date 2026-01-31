using FileSync.Domain.entities;
using FileSync.Domain.entities;
using FileSync.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FileSync.Infrastructure.Persistence.Repositories;

public class SpaceRepository : ISpaceRepository
{
    private readonly AppDbContext _context;

    public SpaceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Space?> GetByIdAsync(Guid id)
    {
        return await _context.Spaces
            .Include(s => s.Files)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Space?> GetByCodeAsync(string code)
    {
        return await _context.Spaces
            .Include(s => s.Files)
            .FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task AddAsync(Space space)
    {
        await _context.Spaces.AddAsync(space);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Space space)
    {
        _context.Spaces.Update(space);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var space = await _context.Spaces.FindAsync(id);
        if (space != null)
        {
            _context.Spaces.Remove(space);
            await _context.SaveChangesAsync();
        }
    }


    public async Task AddFileAsync(SpaceFile file)
    {
        await _context.SpaceFiles.AddAsync(file);
        await _context.SaveChangesAsync();
    }
}