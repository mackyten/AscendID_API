using AscendID_API.Application.Commands;
using AscendID_API.Application.Queries;
using AscendID_API.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AscendID_API.BlazorUI.Services;

/// <summary>
/// Service for handling TempTestRecord operations.
/// This demonstrates Blazor Server mode with direct Application layer access.
/// For API mode, you would use HttpClient instead.
/// </summary>
public class TempTestRecordService
{
    private readonly IMediator _mediator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TempTestRecordService> _logger;

    public TempTestRecordService(
        IMediator mediator,
        IHttpClientFactory httpClientFactory,
        ILogger<TempTestRecordService> logger)
    {
        _mediator = mediator;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    // Direct Application Access (Blazor Server Mode)
    public async Task<List<TempTestRecordDto>> GetAllRecordsAsync()
    {
        try
        {
            var query = new GetAllTempTestRecordsQuery();
            var result = await _mediator.Send(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all records");
            throw;
        }
    }

    public async Task<TempTestRecordDto?> GetRecordByIdAsync(int id)
    {
        try
        {
            var query = new GetTempTestRecordByIdQuery(id);
            return await _mediator.Send(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting record by id {Id}", id);
            throw;
        }
    }

    public async Task<int> CreateRecordAsync(CreateTempTestRecordDto dto)
    {
        try
        {
            var command = new CreateTempTestRecordCommand(dto.Name, dto.Description);
            var result = await _mediator.Send(command);
            return result.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating record");
            throw;
        }
    }

    public async Task UpdateRecordAsync(int id, UpdateTempTestRecordDto dto)
    {
        try
        {
            var command = new UpdateTempTestRecordCommand(id, dto.Name, dto.Description);
            await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating record {Id}", id);
            throw;
        }
    }

    public async Task DeleteRecordAsync(int id)
    {
        try
        {
            var command = new DeleteTempTestRecordCommand(id);
            await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting record {Id}", id);
            throw;
        }
    }

    // Alternative: WebAPI Mode (using HttpClient)
    public async Task<List<TempTestRecordDto>> GetAllRecordsFromApiAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AscendIDAPI");
            var response = await client.GetFromJsonAsync<List<TempTestRecordDto>>("/api/temptestrecords");
            return response ?? new List<TempTestRecordDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all records from API");
            throw;
        }
    }

    public async Task<TempTestRecordDto?> GetRecordByIdFromApiAsync(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AscendIDAPI");
            return await client.GetFromJsonAsync<TempTestRecordDto>($"/api/temptestrecords/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting record by id {Id} from API", id);
            throw;
        }
    }

    public async Task<int> CreateRecordFromApiAsync(CreateTempTestRecordDto dto)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AscendIDAPI");
            var response = await client.PostAsJsonAsync("/api/temptestrecords", dto);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TempTestRecordDto>();
            return result?.Id ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating record from API");
            throw;
        }
    }
}
