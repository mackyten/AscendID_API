using AscendID_API.Application.DTOs;
using MediatR;

namespace AscendID_API.Application.Queries;

public record GetTempTestRecordByIdQuery(int Id) : IRequest<TempTestRecordDto?>;