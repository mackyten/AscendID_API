using AscendID_API.Domain.Entities;

namespace AscendID_API.Application.Interfaces;

public interface ITempTestRecordRepository
{
    Task<TempTestRecord?> GetByIdAsync(int id);
    Task<IEnumerable<TempTestRecord>> GetAllAsync();
    Task<TempTestRecord> CreateAsync(TempTestRecord record);
    Task<TempTestRecord> UpdateAsync(TempTestRecord record);
    Task<bool> DeleteAsync(int id);
}
