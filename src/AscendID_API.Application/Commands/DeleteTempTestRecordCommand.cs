using MediatR;

namespace AscendID_API.Application.Commands;

public record DeleteTempTestRecordCommand(int Id) : IRequest<bool>;