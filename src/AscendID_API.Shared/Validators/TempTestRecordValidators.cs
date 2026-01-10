using FluentValidation;
using AscendID_API.Application.DTOs;

namespace AscendID_API.Shared.Validators;

/// <summary>
/// FluentValidation validator for CreateTempTestRecordDto.
/// This validator can be shared between the WebAPI and BlazorUI projects
/// to ensure consistent validation rules across the application.
/// </summary>
public class CreateTempTestRecordDtoValidator : AbstractValidator<CreateTempTestRecordDto>
{
    public CreateTempTestRecordDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(1, 100)
            .WithMessage("Name must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Name can only contain letters, numbers, spaces, hyphens, and underscores.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

/// <summary>
/// FluentValidation validator for UpdateTempTestRecordDto.
/// This validator can be shared between the WebAPI and BlazorUI projects
/// to ensure consistent validation rules across the application.
/// </summary>
public class UpdateTempTestRecordDtoValidator : AbstractValidator<UpdateTempTestRecordDto>
{
    public UpdateTempTestRecordDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(1, 100)
            .WithMessage("Name must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Name can only contain letters, numbers, spaces, hyphens, and underscores.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
