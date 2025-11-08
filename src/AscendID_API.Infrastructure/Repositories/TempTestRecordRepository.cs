using Microsoft.EntityFrameworkCore;
using AscendID_API.Application.Interfaces;
using AscendID_API.Domain.Entities;
using AscendID_API.Infrastructure.Data;

namespace AscendID_API.Infrastructure.Repositories;

public class TempTestRecordRepository : ITempTestRecordRepository
{
    private readonly ApplicationDbContext _context;

    public TempTestRecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TempTestRecord?> GetByIdAsync(int id)
    {
        return await _context.TempTestRecords.FindAsync(id);
    }

    public async Task<IEnumerable<TempTestRecord>> GetAllAsync()
    {
        return await _context.TempTestRecords
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<TempTestRecord> CreateAsync(TempTestRecord record)
    {
        _context.TempTestRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<TempTestRecord> UpdateAsync(TempTestRecord record)
    {
        _context.TempTestRecords.Update(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _context.TempTestRecords.FindAsync(id);
        if (record == null)
            return false;

        // Soft delete by marking as inactive
        record.Deactivate();
        await _context.SaveChangesAsync();
        return true;
    }
}