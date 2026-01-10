# Integrating Shared FluentValidation in WebAPI

This guide demonstrates how to integrate the shared FluentValidation validators from `AscendID_API.Shared` into your WebAPI project.

## Step 1: Add Project Reference

Add a reference to the Shared project in your WebAPI `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\AscendID_API.Shared\AscendID_API.Shared.csproj" />
</ItemGroup>
```

## Step 2: Register Validators

In your `Program.cs`, register the validators:

```csharp
using AscendID_API.Shared.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateTempTestRecordDtoValidator>();

// Or register all validators from the Shared assembly
builder.Services.AddValidatorsFromAssembly(typeof(CreateTempTestRecordDtoValidator).Assembly);

// Add FluentValidation to MVC
builder.Services.AddControllers()
    .AddFluentValidation(fv => 
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateTempTestRecordDtoValidator>();
    });
```

## Step 3: Use Validators in Controllers

### Option 1: Automatic Validation with Model State

```csharp
using Microsoft.AspNetCore.Mvc;
using AscendID_API.Application.DTOs;
using FluentValidation;

namespace AscendID_API.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TempTestRecordsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTempTestRecordDto dto)
    {
        // With FluentValidation integrated into MVC, validation happens automatically
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        // Process the valid request...
        return Ok();
    }
}
```

### Option 2: Manual Validation with IValidator

```csharp
using Microsoft.AspNetCore.Mvc;
using AscendID_API.Application.DTOs;
using FluentValidation;

namespace AscendID_API.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TempTestRecordsController : ControllerBase
{
    private readonly IValidator<CreateTempTestRecordDto> _createValidator;
    private readonly IValidator<UpdateTempTestRecordDto> _updateValidator;

    public TempTestRecordsController(
        IValidator<CreateTempTestRecordDto> createValidator,
        IValidator<UpdateTempTestRecordDto> updateValidator)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTempTestRecordDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Errors = validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Message = e.ErrorMessage
                })
            });
        }
        
        // Process the valid request...
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTempTestRecordDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Errors = validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Message = e.ErrorMessage
                })
            });
        }
        
        // Process the valid request...
        return Ok();
    }
}
```

### Option 3: Custom Validation Filter

Create a reusable validation filter:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AscendID_API.WebAPI.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}
```

Apply it to your controllers:

```csharp
[ApiController]
[Route("api/[controller]")]
[ValidateModel]  // Apply the filter
public class TempTestRecordsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTempTestRecordDto dto)
    {
        // Validation is already done by the filter
        // Process the valid request...
        return Ok();
    }
}
```

## Step 4: Testing the Integration

### Test with Valid Data

```bash
curl -X POST https://localhost:7001/api/temptestrecords \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Valid Name",
    "description": "Valid description"
  }'
```

### Test with Invalid Data

```bash
curl -X POST https://localhost:7001/api/temptestrecords \
  -H "Content-Type: application/json" \
  -d '{
    "name": "",
    "description": "Too long description..." (over 500 chars)
  }'
```

Expected response:
```json
{
  "errors": [
    {
      "property": "Name",
      "message": "Name is required."
    },
    {
      "property": "Description",
      "message": "Description cannot exceed 500 characters."
    }
  ]
}
```

## Benefits

✅ **Shared Validation**: Same rules in API and UI
✅ **Consistent Error Messages**: Users see the same messages everywhere
✅ **Reduced Code Duplication**: Write validators once, use them everywhere
✅ **Easier Maintenance**: Update validation in one place
✅ **Better Testing**: Test validators independently from controllers

## Required NuGet Packages

Add these packages to your WebAPI project:

```xml
<PackageReference Include="FluentValidation" Version="11.9.0" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
```

## Complete Example

See the full implementation in:
- **Validators**: `src/AscendID_API.Shared/Validators/TempTestRecordValidators.cs`
- **BlazorUI Usage**: `src/AscendID_API.BlazorUI/Components/Pages/CreateRecord.razor`
- **WebAPI Usage**: Update your controllers to inject and use the validators as shown above
