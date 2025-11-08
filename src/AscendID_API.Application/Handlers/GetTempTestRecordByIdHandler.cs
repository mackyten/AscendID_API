using AscendID_API.Application.DTOs;
using AscendID_API.Application.Interfaces;
using AscendID_API.Application.Queries;
using MediatR;

namespace AscendID_API.Application.Handlers;

public class GetTempTestRecordByIdHandler : IRequestHandler<GetTempTestRecordByIdQuery, TempTestRecordDto?>
{
    private readonly ITempTestRecordRepository _repository;

    public GetTempTestRecordByIdHandler(ITempTestRecordRepository repository)
    {
        _repository = repository;
    }

    public async Task<TempTestRecordDto?> Handle(GetTempTestRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        
        return entity == null ? null : new TempTestRecordDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.IsActive
        );
    }
}