# AscendID API - Complete Architecture Overview

## Project Structure

The AscendID API solution consists of 6 projects organized into a Clean Architecture pattern:

```
AscendID_API/
├── src/
│   ├── AscendID_API.Domain/              # Core domain entities and interfaces
│   ├── AscendID_API.Application/         # Business logic, DTOs, handlers
│   ├── AscendID_API.Infrastructure/      # Database, repositories, EF Core
│   ├── AscendID_API.Shared/              # Shared validators and utilities
│   ├── AscendID_API.WebAPI/              # REST API endpoints (ASP.NET Core)
│   └── AscendID_API.BlazorUI/            # Blazor Server UI
├── AscendID_API.sln                      # Solution file
└── [Documentation Files]
```

## Projects Overview

### 1. **AscendID_API.Domain** (.NET 8.0)
- **Purpose**: Core domain entities and business rules
- **Contains**:
  - Entity definitions (e.g., `TempTestRecord`)
  - Repository interfaces
  - Domain exceptions and value objects
- **Dependencies**: None (only NuGet packages)

### 2. **AscendID_API.Application** (.NET 8.0)
- **Purpose**: Application layer with business logic using CQRS pattern
- **Contains**:
  - Commands (CQRS mutation operations)
  - Queries (CQRS read operations)
  - Handlers (MediatR request handlers)
  - DTOs (Data Transfer Objects)
  - Repository interfaces
- **Pattern**: CQRS + MediatR
- **Dependencies**: Domain, MediatR

### 3. **AscendID_API.Infrastructure** (.NET 8.0)
- **Purpose**: Data access and external services
- **Contains**:
  - Entity Framework Core `DbContext`
  - Repository implementations
  - Database migrations
  - Connection to Domain services
- **Dependencies**: Domain, Application
- **Databases**: PostgreSQL

### 4. **AscendID_API.Shared** (.NET 8.0) ⭐ **NEW**
- **Purpose**: Shared code between WebAPI and BlazorUI
- **Contains**:
  - FluentValidation validators
  - Shared constants
  - Utility functions
- **Benefits**: Single source of truth for validation rules
- **Dependencies**: Application

### 5. **AscendID_API.WebAPI** (.NET 8.0)
- **Purpose**: REST API endpoints
- **Contains**:
  - Controllers (HTTP endpoints)
  - Authentication & Authorization
  - Swagger/OpenAPI documentation
  - Dependency injection configuration
- **Features**:
  - JWT authentication
  - OpenIddict OAuth 2.0
  - Swagger UI for API documentation
- **Dependencies**: Application, Infrastructure, Shared

### 6. **AscendID_API.BlazorUI** (.NET 8.0) ⭐ **NEW**
- **Purpose**: Blazor Server UI for the application
- **Contains**:
  - Razor components and pages
  - Services for business logic
  - Layout and navigation
- **Features**:
  - Real-time validation with Blazored.FluentValidation
  - Two operation modes (Blazor Server or API)
  - Responsive Bootstrap-based UI
- **Dependencies**: Application, Infrastructure, Shared

## Key Architectural Patterns

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, List)
- **Handler**: `IRequestHandler<TRequest, TResponse>` with MediatR
- **Benefits**: Clear separation of concerns, easier to test, scales well

### Dependency Injection
All projects use Microsoft Dependency Injection configured in `Program.cs`:
```csharp
// Register repositories
builder.Services.AddScoped<ITempTestRecordRepository, TempTestRecordRepository>();

// Register MediatR for CQRS
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(TempTestRecordDto).Assembly)
);

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateTempTestRecordDtoValidator>();
```

### Shared Validation ⭐ **KEY FEATURE**
The **AscendID_API.Shared** project provides FluentValidation validators that are used by both the WebAPI and BlazorUI:

**Validator Definition** (in Shared project):
```csharp
public class CreateTempTestRecordDtoValidator : AbstractValidator<CreateTempTestRecordDto>
{
    public CreateTempTestRecordDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(1, 100).WithMessage("Name must be 1-100 characters.");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description max 500 chars.");
    }
}
```

**Used in WebAPI**:
```csharp
var validationResult = await _validator.ValidateAsync(dto);
if (!validationResult.IsValid)
    return BadRequest(validationResult.Errors);
```

**Used in BlazorUI**:
```razor
<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <FluentValidationValidator />
    <InputText @bind-Value="model.Name" />
    <ValidationMessage For="@(() => model.Name)" />
</EditForm>
```

## Data Flow

### Create Record Flow (Blazor Server Mode)
```
User Input (Razor Component)
    ↓
EditForm + FluentValidationValidator (Client-side validation)
    ↓
OnValidSubmit handler
    ↓
TempTestRecordService.CreateRecordAsync()
    ↓
MediatR.Send(CreateTempTestRecordCommand)
    ↓
CreateTempTestRecordHandler
    ↓
ITempTestRecordRepository.AddAsync()
    ↓
Entity Framework Core
    ↓
PostgreSQL Database
```

### Create Record Flow (API Mode)
```
User Input (Razor Component)
    ↓
EditForm + FluentValidationValidator (Client-side validation)
    ↓
OnValidSubmit handler
    ↓
TempTestRecordService.CreateRecordFromApiAsync()
    ↓
HttpClient.PostAsJsonAsync() → WebAPI
    ↓
WebAPI Controller receives request
    ↓
Server-side validation (FluentValidation - same validator!)
    ↓
MediatR.Send(CreateTempTestRecordCommand)
    ↓
[Same flow as above...]
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- PostgreSQL database
- Visual Studio Code or Visual Studio

### Setup

1. **Clone/Open the project**
```bash
cd C:\Users\marka\personal_projects\AscendID_API
```

2. **Restore packages**
```bash
dotnet restore
```

3. **Configure database**
- Update connection string in `appsettings.json` for WebAPI and BlazorUI
- Run migrations: `dotnet ef database update --project src/AscendID_API.Infrastructure`

4. **Run WebAPI**
```bash
dotnet run --project src/AscendID_API.WebAPI
# Runs on https://localhost:7001
# Swagger UI: https://localhost:7001/swagger
```

5. **Run BlazorUI** (in separate terminal)
```bash
dotnet run --project src/AscendID_API.BlazorUI
# Runs on https://localhost:7002
```

## Testing the Shared Validation

### Test 1: Invalid Data
```bash
# Should fail validation in both API and UI
curl -X POST https://localhost:7001/api/temptestrecords \
  -H "Content-Type: application/json" \
  -d '{"name": "","description": "A"}'
```

### Test 2: Valid Data
```bash
# Should pass validation in both API and UI
curl -X POST https://localhost:7001/api/temptestrecords \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Record","description": "A test description"}'
```

## Adding New Features

### Example: Add a new validator

1. **Create validator in Shared project**:
```csharp
// src/AscendID_API.Shared/Validators/NewDtoValidator.cs
public class NewDtoValidator : AbstractValidator<NewDto>
{
    public NewDtoValidator()
    {
        RuleFor(x => x.Property).NotEmpty();
    }
}
```

2. **Register in WebAPI** (`Program.cs`):
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<NewDtoValidator>();
```

3. **Register in BlazorUI** (`Program.cs`):
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<NewDtoValidator>();
```

4. **Use in both projects** - automatically picked up!

## Project Dependencies

```
Domain (0 dependencies)
    ↑
Application (depends on Domain)
    ↑
Infrastructure (depends on Domain + Application)
    ↑
Shared (depends on Application)
    ↑
WebAPI (depends on Application + Infrastructure + Shared)
BlazorUI (depends on Application + Infrastructure + Shared)
```

## Technologies Used

| Component | Technology |
|-----------|-----------|
| Backend | ASP.NET Core 8.0 |
| UI Framework | Blazor Server |
| Database | PostgreSQL 15+ |
| ORM | Entity Framework Core 9.0 |
| CQRS | MediatR |
| Validation | FluentValidation |
| Blazor Validation | Blazored.FluentValidation |
| Authentication | JWT + OpenIddict |
| API Documentation | Swagger/OpenAPI |

## Documentation Files

- **[SHARED_VALIDATION_GUIDE.md](SHARED_VALIDATION_GUIDE.md)** - How to use shared validators in WebAPI
- **[src/AscendID_API.Shared/README.md](src/AscendID_API.Shared/README.md)** - Shared project details
- **[src/AscendID_API.BlazorUI/README.md](src/AscendID_API.BlazorUI/README.md)** - BlazorUI documentation
- **[README.md](README.md)** - Original project README

## Clean Code Principles Applied

1. ✅ **Single Responsibility Principle**: Each project has one responsibility
2. ✅ **Open/Closed Principle**: Open for extension (validators), closed for modification
3. ✅ **Liskov Substitution**: Repositories implement interfaces
4. ✅ **Interface Segregation**: Small, focused interfaces
5. ✅ **Dependency Inversion**: Depend on abstractions, not concretions
6. ✅ **DRY (Don't Repeat Yourself)**: Shared validators eliminate duplication
7. ✅ **SOLID Principles**: All followed throughout the architecture

## Next Steps

1. Implement authentication in BlazorUI
2. Add unit tests for validators and handlers
3. Implement pagination for large datasets
4. Add logging and monitoring
5. Optimize database queries with lazy loading
6. Add caching layer
