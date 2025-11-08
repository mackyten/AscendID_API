using AscendID_API.Application.Commands;
using AscendID_API.Application.DTOs;
using AscendID_API.Application.Interfaces;
using MediatR;

namespace AscendID_API.Application.Handlers;

public class UpdateTempTestRecordHandler : IRequestHandler<UpdateTempTestRecordCommand, TempTestRecordDto>
{
    private readonly ITempTestRecordRepository _repository;

    public UpdateTempTestRecordHandler(ITempTestRecordRepository repository)
    {
        _repository = repository;
    }

    public async Task<TempTestRecordDto> Handle(UpdateTempTestRecordCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"TempTestRecord with ID {request.Id} not found.");
        }

        entity.Update(request.Name, request.Description);
        var updated = await _repository.UpdateAsync(entity);

        return new TempTestRecordDto(
            updated.Id,
            updated.Name,
            updated.Description,
            updated.CreatedAt,
            updated.UpdatedAt,
            updated.IsActive
        );
    }
}