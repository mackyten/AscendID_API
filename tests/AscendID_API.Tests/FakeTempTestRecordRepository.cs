using AscendID_API.Application.Interfaces;
using AscendID_API.Domain.Entities;

namespace AscendID_API.Tests;

/// <summary>In-memory repository so handlers can be tested without a database.</summary>
public class FakeTempTestRecordRepository : ITempTestRecordRepository
{
    private readonly Dictionary<int, TempTestRecord> items = new();
    private int nextId = 1;

    public Task<TempTestRecord?> GetByIdAsync(int id) =>
        Task.FromResult(items.TryGetValue(id, out var record) ? record : null);

    public Task<IEnumerable<TempTestRecord>> GetAllAsync() =>
        Task.FromResult<IEnumerable<TempTestRecord>>(items.Values.ToList());

    public Task<TempTestRecord> CreateAsync(TempTestRecord record)
    {
        record.Id = nextId++;
        items[record.Id] = record;
        return Task.FromResult(record);
    }

    public Task<TempTestRecord> UpdateAsync(TempTestRecord record)
    {
        items[record.Id] = record;
        return Task.FromResult(record);
    }

    public Task<bool> DeleteAsync(int id) => Task.FromResult(items.Remove(id));
}
