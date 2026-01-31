using FileSync.Domain.entities;
using FileSync.Domain.enums;

using FileSync.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FileSync.Infrastructure.Persistence.Repositories;

public class DropRepository : IFileDropRepository
{
    private readonly AppDbContext _context;

    public DropRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FileDrop?> GetByIdAsync(Guid id)
    {
        return await _context.Drops
            .Include(d => d.Files)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<FileDrop?> GetByCodeAsync(string code)
    {
        return await _context.Drops
            .Include(d => d.Files)
            .FirstOrDefaultAsync(d => d.Code == code);
    }

    public async Task AddAsync(FileDrop drop)
    {
        await _context.Drops.AddAsync(drop);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FileDrop drop)
    {
        _context.Drops.Update(drop);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var drop = await _context.Drops.FindAsync(id);
        if (drop != null)
        {
            _context.Drops.Remove(drop);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<FileDrop>> GetExpiredDropsAsync()
    {
        return await _context.Drops
            .Include(d => d.Files)
            .Where(d => d.ExpiresAt < DateTime.UtcNow && d.Status == DropStatus.Active)
            .ToListAsync();
    }


    public async Task AddFileAsync(DropFile file)
    {
        await _context.DropFiles.AddAsync(file);
        await _context.SaveChangesAsync();
    }
}
