# Quick Reference: Shared Validation in AscendID API

## 🎯 Overview
The AscendID API now has a **Shared Validation Layer** that ensures consistent validation rules across the WebAPI and BlazorUI applications.

## 📁 Key Files

### Validators (Shared Layer)
- **Path**: `src/AscendID_API.Shared/Validators/TempTestRecordValidators.cs`
- **Classes**:
  - `CreateTempTestRecordDtoValidator`
  - `UpdateTempTestRecordDtoValidator`

### BlazorUI Usage
- **Service**: `src/AscendID_API.BlazorUI/Services/TempTestRecordService.cs`
- **Pages**: 
  - `Components/Pages/CreateRecord.razor` (with validation)
  - `Components/Pages/EditRecord.razor` (with validation)
  - `Components/Pages/TempTestRecords.razor` (list view)

### WebAPI (Ready for Integration)
- **Reference**: See `SHARED_VALIDATION_GUIDE.md` for integration steps
- **Status**: Ready to implement in controllers

## 🔧 How to Use Shared Validators

### In Blazor UI (Already Implemented)
```razor
<!-- Components/Pages/CreateRecord.razor -->
<EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
    <FluentValidationValidator />
    <InputText id="name" @bind-Value="model.Name" />
    <ValidationMessage For="@(() => model.Name)" />
</EditForm>
```

The validator automatically:
- Validates on blur
- Validates on submit
- Shows error messages
- Prevents invalid submissions

### In WebAPI (Integration Pattern)
```csharp
// In Program.cs
builder.Services.AddValidatorsFromAssemblyContaining<CreateTempTestRecordDtoValidator>();

// In Controller
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateTempTestRecordDto dto)
{
    var result = await _validator.ValidateAsync(dto);
    if (!result.IsValid)
        return BadRequest(result.Errors);
    // Process...
}
```

## 📋 Validation Rules

### For CreateTempTestRecordDto
```
Name:
  - Required ✓
  - Length: 1-100 characters
  - Pattern: ^[a-zA-Z0-9\s\-_]+$ (alphanumeric, spaces, hyphens, underscores)

Description:
  - Optional
  - Max length: 500 characters
```

### For UpdateTempTestRecordDto
```
Same rules as CreateTempTestRecordDto
```

## 🚀 Running the Application

### Terminal 1: Start WebAPI
```bash
cd C:\Users\marka\personal_projects\AscendID_API
dotnet run --project src/AscendID_API.WebAPI
# Runs on: https://localhost:7001
# Swagger: https://localhost:7001/swagger
```

### Terminal 2: Start BlazorUI
```bash
cd C:\Users\marka\personal_projects\AscendID_API
dotnet run --project src/AscendID_API.BlazorUI
# Runs on: https://localhost:7002
```

## 🧪 Testing Validation

### Test 1: Invalid Name (Empty)
```json
{
  "name": "",
  "description": "Test"
}
```
**Expected**: Error - "Name is required."

### Test 2: Invalid Name (Too Long)
```json
{
  "name": "This name is way too long and exceeds the 100 character limit that we have set",
  "description": "Test"
}
```
**Expected**: Error - "Name must be between 1 and 100 characters."

### Test 3: Invalid Description (Too Long)
```json
{
  "name": "Valid Name",
  "description": "..." (> 500 chars)
}
```
**Expected**: Error - "Description cannot exceed 500 characters."

### Test 4: Valid Data
```json
{
  "name": "Valid Name",
  "description": "Valid description"
}
```
**Expected**: Success ✓

## 📊 Project Structure

```
AscendID_API/
├── src/
│   ├── AscendID_API.Domain/              # Core entities
│   ├── AscendID_API.Application/         # CQRS + Handlers
│   ├── AscendID_API.Infrastructure/      # Database + EF Core
│   ├── AscendID_API.Shared/ ⭐ NEW       # Shared validators
│   ├── AscendID_API.WebAPI/              # REST API
│   └── AscendID_API.BlazorUI/ ⭐ NEW     # Blazor UI
│
├── Documentation/
│   ├── ARCHITECTURE.md                   # Full architecture guide
│   ├── BLAZOR_IMPLEMENTATION_SUMMARY.md  # BlazorUI details
│   ├── SHARED_VALIDATION_GUIDE.md        # API integration guide
│   └── CONFIGURATION.md
│
└── AscendID_API.sln
```

## 🔄 Data Flow: Create Record

### Client Side (BlazorUI)
```
User fills form
    ↓
FluentValidationValidator validates (client-side)
    ↓
User clicks submit
    ↓
OnValidSubmit triggers
    ↓
TempTestRecordService.CreateRecordAsync()
```

### Server Side (Blazor Server Mode)
```
Service calls MediatR
    ↓
MediatR routes to Handler
    ↓
Handler calls Repository
    ↓
Repository saves to Database
    ↓
Response returned to UI
```

### Server Side (API Mode)
```
Service calls HttpClient
    ↓
WebAPI receives request
    ↓
Server-side validation (same validator!)
    ↓
Controller processes request
    ↓
MediatR → Handler → Repository → Database
```

## 📚 Documentation Files

| File | Purpose |
|------|---------|
| `ARCHITECTURE.md` | Complete system design and architecture |
| `BLAZOR_IMPLEMENTATION_SUMMARY.md` | BlazorUI layer details and features |
| `SHARED_VALIDATION_GUIDE.md` | How to integrate validators in WebAPI |
| `src/AscendID_API.Shared/README.md` | Shared project documentation |
| `src/AscendID_API.BlazorUI/README.md` | BlazorUI project documentation |

## ✨ Key Benefits

✅ **Single Source of Truth**: Validators defined once, used everywhere
✅ **Type Safe**: Compile-time checking ensures validators match DTOs
✅ **Consistent UX**: Users see same validation messages everywhere
✅ **Easy Maintenance**: Update validation in one place
✅ **DRY Principle**: No code duplication
✅ **Professional**: Follows industry best practices
✅ **Testable**: Validators can be unit tested independently
✅ **Extensible**: Easy to add new validators

## 🛠️ Adding a New Validator

1. **Create validator in Shared project**:
```csharp
// src/AscendID_API.Shared/Validators/YourDtoValidator.cs
public class YourDtoValidator : AbstractValidator<YourDto>
{
    public YourDtoValidator()
    {
        RuleFor(x => x.Property)
            .NotEmpty().WithMessage("Required");
    }
}
```

2. **Register in both projects** (Program.cs):
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<YourDtoValidator>();
```

3. **Use in components** (Blazor):
```razor
<EditForm Model="@model">
    <FluentValidationValidator />
    <!-- Form fields -->
</EditForm>
```

4. **Use in controllers** (WebAPI):
```csharp
var result = await _validator.ValidateAsync(dto);
```

## 🎓 Learning Resources

- **FluentValidation**: https://docs.fluentvalidation.net/
- **Blazor Documentation**: https://learn.microsoft.com/aspnet/core/blazor
- **Clean Architecture**: https://blog.cleancoder.com/
- **CQRS Pattern**: https://martinfowler.com/bliki/CQRS.html

## ⚡ Quick Commands

```bash
# Build solution
dotnet build AscendID_API.sln

# Build specific project
dotnet build src/AscendID_API.BlazorUI/AscendID_API.BlazorUI.csproj

# Run tests (when added)
dotnet test

# Run with watch
dotnet watch run --project src/AscendID_API.BlazorUI

# Apply migrations
dotnet ef database update --project src/AscendID_API.Infrastructure

# Add new migration
dotnet ef migrations add MigrationName --project src/AscendID_API.Infrastructure
```

## 🔗 Project Dependencies

```
Shared project imports from:
  - AscendID_API.Application (for DTOs)
  - FluentValidation

BlazorUI imports from:
  - AscendID_API.Application
  - AscendID_API.Infrastructure
  - AscendID_API.Shared (validators)

WebAPI imports from:
  - AscendID_API.Application
  - AscendID_API.Infrastructure
  - AscendID_API.Shared (validators)
```

## 🎯 Next Steps

1. ✅ Review ARCHITECTURE.md for system overview
2. ✅ Review BLAZOR_IMPLEMENTATION_SUMMARY.md for UI details
3. ⏳ Implement WebAPI controller integration (SHARED_VALIDATION_GUIDE.md)
4. ⏳ Add authentication/authorization
5. ⏳ Create unit tests
6. ⏳ Add additional validators as needed
7. ⏳ Implement pagination and search

---

**Version**: 1.0
**Last Updated**: January 10, 2026
**Status**: ✅ Complete and Building Successfully
