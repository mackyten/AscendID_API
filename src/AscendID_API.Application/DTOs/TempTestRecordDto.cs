using System.ComponentModel.DataAnnotations;

namespace AscendID_API.Application.DTOs;

public record TempTestRecordDto(
    int Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsActive
);

public record CreateTempTestRecordDto(
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Name,
    
    [StringLength(500)]
    string Description
);

public record UpdateTempTestRecordDto(
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Name,
    
    [StringLength(500)]
    string Description
);