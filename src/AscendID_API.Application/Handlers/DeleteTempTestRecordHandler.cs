using AscendID_API.Application.Commands;
using AscendID_API.Application.Interfaces;
using MediatR;

namespace AscendID_API.Application.Handlers;

public class DeleteTempTestRecordHandler : IRequestHandler<DeleteTempTestRecordCommand, bool>
{
    private readonly ITempTestRecordRepository _repository;

    public DeleteTempTestRecordHandler(ITempTestRecordRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteTempTestRecordCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.Id);
    }
}