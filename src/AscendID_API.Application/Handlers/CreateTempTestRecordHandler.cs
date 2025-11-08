using AscendID_API.Application.Commands;
using AscendID_API.Application.DTOs;
using AscendID_API.Application.Interfaces;
using AscendID_API.Domain.Entities;
using MediatR;

namespace AscendID_API.Application.Handlers;

public class CreateTempTestRecordHandler : IRequestHandler<CreateTempTestRecordCommand, TempTestRecordDto>
{
    private readonly ITempTestRecordRepository _repository;

    public CreateTempTestRecordHandler(ITempTestRecordRepository repository)
    {
        _repository = repository;
    }

    public async Task<TempTestRecordDto> Handle(CreateTempTestRecordCommand request, CancellationToken cancellationToken)
    {
        var entity = new TempTestRecord
        {
            Name = request.Name,
            Description = request.Description
        };

        var created = await _repository.CreateAsync(entity);

        return new TempTestRecordDto(
            created.Id,
            created.Name,
            created.Description,
            created.CreatedAt,
            created.UpdatedAt,
            created.IsActive
        );
    }
}