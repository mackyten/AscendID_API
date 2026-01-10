# AscendID API - Blazor UI

This is the Blazor Server UI for the AscendID API project, demonstrating two modes of operation:

1. **Blazor Server Mode**: Direct access to the Application layer via MediatR
2. **API Mode**: HTTP calls to the WebAPI endpoints

## Features

- ✅ CRUD operations for Temp Test Records
- ✅ Shared FluentValidation between API and UI
- ✅ Real-time validation feedback
- ✅ Responsive Bootstrap-based UI
- ✅ Support for both direct Application access and WebAPI calls

## Shared Validation

This project demonstrates "Clean Code" consistency by sharing FluentValidation validators between the WebAPI and Blazor UI through the **AscendID_API.Shared** project.

### How It Works

1. **Shared Validators**: FluentValidation classes are defined in `AscendID_API.Shared/Validators/`
2. **API Integration**: The WebAPI references the Shared project and uses the validators
3. **UI Integration**: The Blazor UI references the Shared project and uses the same validators
4. **Consistency**: Changes to validation rules are automatically applied to both API and UI

### Example Validator

```csharp
// Located in: AscendID_API.Shared/Validators/TempTestRecordValidators.cs
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
```

## Project Structure

```
AscendID_API.BlazorUI/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor          # Main layout component
│   │   └── NavMenu.razor             # Navigation menu
│   ├── Pages/
│   │   ├── Home.razor                # Home page
│   │   ├── TempTestRecords.razor     # List all records
│   │   ├── CreateRecord.razor        # Create new record with validation
│   │   └── EditRecord.razor          # Edit existing record
│   ├── App.razor                     # Root component
│   └── Routes.razor                  # Routing configuration
├── Services/
│   └── TempTestRecordService.cs      # Business logic service
├── wwwroot/
│   └── app.css                       # Custom styles
├── Program.cs                        # Application entry point
├── _Imports.razor                    # Global using statements
└── appsettings.json                  # Configuration
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- PostgreSQL database (for Blazor Server mode)
- Running WebAPI instance (for API mode)

### Configuration

1. Update `appsettings.json` with your database connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ascendid_api;Username=postgres;Password=your_password"
  },
  "ApiSettings": {
    "BaseUrl": "https://localhost:7001"
  }
}
```

2. The application can run in two modes:

#### Blazor Server Mode (Direct Application Access)
The service layer uses MediatR to call Application handlers directly:

```csharp
// In TempTestRecordService.cs
public async Task<List<TempTestRecordDto>> GetAllRecordsAsync()
{
    var query = new GetAllTempTestRecordsQuery();
    return await _mediator.Send(query);
}
```

#### API Mode (HTTP Client)
The service layer makes HTTP calls to the WebAPI:

```csharp
// In TempTestRecordService.cs
public async Task<List<TempTestRecordDto>> GetAllRecordsFromApiAsync()
{
    var client = _httpClientFactory.CreateClient("AscendIDAPI");
    var response = await client.GetFromJsonAsync<List<TempTestRecordDto>>("/api/temptestrecords");
    return response ?? new List<TempTestRecordDto>();
}
```

### Running the Application

1. Restore dependencies:
```bash
dotnet restore
```

2. Run the application:
```bash
dotnet run
```

3. Open your browser and navigate to:
   - HTTPS: https://localhost:7002
   - HTTP: http://localhost:5001

## Using Shared Validators

### In Blazor UI Pages

The validators are automatically applied using the `Blazored.FluentValidation` package:

```razor
<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <FluentValidationValidator />
    
    <div class="mb-3">
        <label for="name" class="form-label">Name</label>
        <InputText id="name" class="form-control" @bind-Value="model.Name" />
        <ValidationMessage For="@(() => model.Name)" />
    </div>
    
    <!-- More fields... -->
</EditForm>
```

### In WebAPI Controllers

To use the shared validators in your WebAPI, reference the Shared project and register the validators:

```csharp
// In Program.cs or Startup.cs
builder.Services.AddValidatorsFromAssemblyContaining<CreateTempTestRecordDtoValidator>();

// In your controller
[ApiController]
[Route("api/[controller]")]
public class TempTestRecordsController : ControllerBase
{
    private readonly IValidator<CreateTempTestRecordDto> _validator;

    public TempTestRecordsController(IValidator<CreateTempTestRecordDto> validator)
    {
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTempTestRecordDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        // Process the request...
    }
}
```

## Benefits of Shared Validation

1. **Single Source of Truth**: Validation rules are defined once and used everywhere
2. **Consistency**: API and UI enforce the same rules
3. **Maintainability**: Changes to validation logic only need to be made in one place
4. **Type Safety**: Compile-time checking ensures validators match their DTOs
5. **Testability**: Validators can be unit tested independently

## Dependencies

- **Microsoft.AspNetCore.Components.WebAssembly.Server**: Blazor Server components
- **FluentValidation**: Validation library
- **Blazored.FluentValidation**: Blazor integration for FluentValidation
- **MediatR**: CQRS pattern implementation (for direct Application access)

## Switching Between Modes

To switch between Blazor Server mode and API mode, simply uncomment the desired method call in your pages:

```csharp
// Blazor Server Mode (direct access)
records = await RecordService.GetAllRecordsAsync();

// API Mode (HTTP calls)
// records = await RecordService.GetAllRecordsFromApiAsync();
```

## Additional Resources

- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [Blazored.FluentValidation](https://github.com/Blazored/FluentValidation)
