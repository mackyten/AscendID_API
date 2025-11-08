using AscendID_API.Application.DTOs;
using AscendID_API.Application.Interfaces;
using AscendID_API.Application.Queries;
using MediatR;

namespace AscendID_API.Application.Handlers;

public class GetAllTempTestRecordsHandler : IRequestHandler<GetAllTempTestRecordsQuery, IEnumerable<TempTestRecordDto>>
{
    private readonly ITempTestRecordRepository _repository;

    public GetAllTempTestRecordsHandler(ITempTestRecordRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TempTestRecordDto>> Handle(GetAllTempTestRecordsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync();
        
        return entities.Select(entity => new TempTestRecordDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.IsActive
        ));
    }
}