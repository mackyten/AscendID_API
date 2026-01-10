# AscendID API - Shared Project

This project contains shared code that is used by both the WebAPI and BlazorUI projects, promoting code reuse and consistency across the application.

## Purpose

The Shared project serves as a **single source of truth** for:

- FluentValidation validators
- Common utilities
- Shared constants
- Cross-cutting concerns

## Current Contents

### Validators

FluentValidation validators for DTOs that are shared between the API and UI:

- `CreateTempTestRecordDtoValidator`
- `UpdateTempTestRecordDtoValidator`

## How to Add New Shared Validators

1. Create a new validator class in `Validators/` folder:

```csharp
using FluentValidation;
using AscendID_API.Application.DTOs;

namespace AscendID_API.Shared.Validators;

public class YourDtoValidator : AbstractValidator<YourDto>
{
    public YourDtoValidator()
    {
        RuleFor(x => x.Property)
            .NotEmpty()
            .WithMessage("Property is required.");
    }
}
```

2. Register the validator in both consuming projects:

**In WebAPI (Program.cs):**
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<YourDtoValidator>();
```

**In BlazorUI (Program.cs):**
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<YourDtoValidator>();
```

## Benefits

✅ **Consistency**: Same validation rules enforced everywhere
✅ **Maintainability**: Single place to update validation logic
✅ **DRY Principle**: Don't Repeat Yourself
✅ **Type Safety**: Compile-time validation of rules
✅ **Testability**: Validators can be unit tested independently

## Dependencies

- **FluentValidation**: Core validation library
- **AscendID_API.Application**: For access to DTOs
