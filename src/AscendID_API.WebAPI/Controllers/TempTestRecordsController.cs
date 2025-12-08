using Microsoft.AspNetCore.Mvc;
using MediatR;
using AscendID_API.Application.Commands;
using AscendID_API.Application.Queries;
using AscendID_API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AscendID_API.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "JWT")]
public class TempTestRecordsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TempTestRecordsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all temp test records
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TempTestRecordDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllTempTestRecordsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get a temp test record by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TempTestRecordDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetTempTestRecordByIdQuery(id));
        
        if (result == null)
            return NotFound($"TempTestRecord with ID {id} not found.");

        return Ok(result);
    }

    /// <summary>
    /// Create a new temp test record
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TempTestRecordDto>> Create([FromBody] CreateTempTestRecordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new CreateTempTestRecordCommand(dto.Name, dto.Description);
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing temp test record
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TempTestRecordDto>> Update(int id, [FromBody] UpdateTempTestRecordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var command = new UpdateTempTestRecordCommand(id, dto.Name, dto.Description);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Delete a temp test record (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteTempTestRecordCommand(id));
        
        if (!result)
            return NotFound($"TempTestRecord with ID {id} not found.");

        return NoContent();
    }
}