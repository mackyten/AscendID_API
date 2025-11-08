using AscendID_API.Application.DTOs;
using MediatR;

namespace AscendID_API.Application.Commands;

public record UpdateTempTestRecordCommand(
    int Id,
    string Name,
    string Description
) : IRequest<TempTestRecordDto>;