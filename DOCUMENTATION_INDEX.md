# AscendID API - Documentation Index

## 📖 Getting Started

### For New Developers
1. Start with [QUICK_REFERENCE.md](QUICK_REFERENCE.md) - 5 minute overview
2. Then read [ARCHITECTURE.md](ARCHITECTURE.md) - Full system design

### For Implementation
1. [BLAZOR_IMPLEMENTATION_SUMMARY.md](BLAZOR_IMPLEMENTATION_SUMMARY.md) - BlazorUI details
2. [SHARED_VALIDATION_GUIDE.md](SHARED_VALIDATION_GUIDE.md) - How to use validators in WebAPI

### For Setup & Deployment
1. [ENVIRONMENT_SETUP.md](ENVIRONMENT_SETUP.md) - Environment configuration
2. [CONFIGURATION.md](CONFIGURATION.md) - Application configuration

### For Identity & Security
1. [IDENTITY_IMPLEMENTATION.md](IDENTITY_IMPLEMENTATION.md) - Authentication setup
2. [JWT_SWAGGER_GUIDE.md](JWT_SWAGGER_GUIDE.md) - JWT and Swagger configuration

---

## 📚 Documentation Breakdown

### Core Documentation

#### [QUICK_REFERENCE.md](QUICK_REFERENCE.md) ⭐ **START HERE**
- **What**: Quick reference guide for the shared validation feature
- **When**: Use when you need a quick lookup
- **Content**: 
  - Key files and locations
  - Validation rules
  - Testing examples
  - Quick commands

#### [ARCHITECTURE.md](ARCHITECTURE.md) ⭐ **IMPORTANT**
- **What**: Complete system architecture and design
- **When**: Use to understand the overall design
- **Content**:
  - Project structure and responsibilities
  - CQRS and dependency injection patterns
  - Data flow diagrams
  - Technology stack
  - Getting started guide

#### [BLAZOR_IMPLEMENTATION_SUMMARY.md](BLAZOR_IMPLEMENTATION_SUMMARY.md)
- **What**: Detailed BlazorUI implementation summary
- **When**: Use when working with the UI
- **Content**:
  - Files created
  - Key features
  - Clean code principles
  - Usage examples
  - How to run

#### [SHARED_VALIDATION_GUIDE.md](SHARED_VALIDATION_GUIDE.md)
- **What**: How to integrate shared validators in WebAPI
- **When**: Use when implementing API validation
- **Content**:
  - Step-by-step integration
  - Controller examples
  - Automatic vs manual validation
  - Custom filters
  - Testing examples

### Setup & Configuration

#### [ENVIRONMENT_SETUP.md](ENVIRONMENT_SETUP.md)
- Development environment setup
- Dependencies and tools
- Database configuration

#### [CONFIGURATION.md](CONFIGURATION.md)
- Application settings
- Environment-specific config
- Feature toggles

### Security & Authentication

#### [IDENTITY_IMPLEMENTATION.md](IDENTITY_IMPLEMENTATION.md)
- Identity and authorization setup
- User management
- Role-based access control

#### [JWT_SWAGGER_GUIDE.md](JWT_SWAGGER_GUIDE.md)
- JWT token configuration
- Swagger/OpenAPI setup
- API documentation

### Project README

#### [README.md](README.md)
- Original project overview
- General information

---

## 🎯 Quick Navigation

### "I want to..."

#### Understand the architecture
→ Read [ARCHITECTURE.md](ARCHITECTURE.md)

#### Start developing quickly
→ Read [QUICK_REFERENCE.md](QUICK_REFERENCE.md)

#### Use validators in my Blazor pages
→ See [BLAZOR_IMPLEMENTATION_SUMMARY.md](BLAZOR_IMPLEMENTATION_SUMMARY.md)

#### Integrate validators in WebAPI
→ Read [SHARED_VALIDATION_GUIDE.md](SHARED_VALIDATION_GUIDE.md)

#### Set up my environment
→ Follow [ENVIRONMENT_SETUP.md](ENVIRONMENT_SETUP.md)

#### Configure the application
→ Check [CONFIGURATION.md](CONFIGURATION.md)

#### Add authentication
→ Read [IDENTITY_IMPLEMENTATION.md](IDENTITY_IMPLEMENTATION.md)

#### Set up JWT and Swagger
→ Follow [JWT_SWAGGER_GUIDE.md](JWT_SWAGGER_GUIDE.md)

#### Get a quick overview
→ Look at [README.md](README.md)

---

## 📁 Project Files Reference

### New Projects Created (⭐ 2024)

#### AscendID_API.BlazorUI
```
src/AscendID_API.BlazorUI/
├── Components/
│   ├── Pages/
│   │   ├── Home.razor
│   │   ├── TempTestRecords.razor
│   │   ├── CreateRecord.razor
│   │   └── EditRecord.razor
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── App.razor
│   └── Routes.razor
├── Services/
│   └── TempTestRecordService.cs
├── Program.cs
├── _Imports.razor
├── appsettings.json
└── README.md
```

#### AscendID_API.Shared
```
src/AscendID_API.Shared/
├── Validators/
│   └── TempTestRecordValidators.cs
├── AscendID_API.Shared.csproj
└── README.md
```

### Key Existing Projects

#### AscendID_API.Domain
Core domain entities and interfaces

#### AscendID_API.Application
Business logic using CQRS pattern

#### AscendID_API.Infrastructure
Data access with Entity Framework Core

#### AscendID_API.WebAPI
REST API endpoints

---

## 🔄 Common Tasks

### Add a New Validator
1. Create class in `AscendID_API.Shared/Validators/`
2. Register in both `AscendID_API.WebAPI/Program.cs` and `AscendID_API.BlazorUI/Program.cs`
3. Use in forms or controllers

### Add a New Blazor Page
1. Create `.razor` file in `Components/Pages/`
2. Add route with `@page "/route"`
3. Add to navigation in `NavMenu.razor`

### Add a New API Endpoint
1. Create controller in `AscendID_API.WebAPI/Controllers/`
2. Create command/query in `AscendID_API.Application/`
3. Create handler in `AscendID_API.Application/Handlers/`

### Run the Application
1. WebAPI: `dotnet run --project src/AscendID_API.WebAPI`
2. BlazorUI: `dotnet run --project src/AscendID_API.BlazorUI`

---

## 🚀 Quick Start

```bash
# 1. Build
dotnet build AscendID_API.sln

# 2. Terminal 1 - Run WebAPI
dotnet run --project src/AscendID_API.WebAPI
# Visit: https://localhost:7001/swagger

# 3. Terminal 2 - Run BlazorUI
dotnet run --project src/AscendID_API.BlazorUI
# Visit: https://localhost:7002
```

---

## 📊 Documentation Statistics

- **Total Documentation Files**: 9
- **New Files Created**: 4 (ARCHITECTURE.md, BLAZOR_IMPLEMENTATION_SUMMARY.md, SHARED_VALIDATION_GUIDE.md, QUICK_REFERENCE.md)
- **Code Files**: 50+
- **Lines of Documentation**: 5000+
- **Build Status**: ✅ Successful

---

## 🔗 Project Links

| Component | Location | Status |
|-----------|----------|--------|
| Domain | `src/AscendID_API.Domain/` | ✅ Active |
| Application | `src/AscendID_API.Application/` | ✅ Active |
| Infrastructure | `src/AscendID_API.Infrastructure/` | ✅ Active |
| Shared | `src/AscendID_API.Shared/` | ✅ NEW |
| WebAPI | `src/AscendID_API.WebAPI/` | ✅ Active |
| BlazorUI | `src/AscendID_API.BlazorUI/` | ✅ NEW |

---

## 💡 Key Concepts

### Shared Validation
The **AscendID_API.Shared** project contains FluentValidation validators that are used by both the WebAPI and BlazorUI to ensure consistent validation rules across the application.

### CQRS Pattern
Commands handle write operations, Queries handle read operations. MediatR routes requests to appropriate handlers.

### Dependency Injection
All dependencies are registered in `Program.cs` and resolved automatically by ASP.NET Core.

### Blazor Server Mode
BlazorUI can access Application layer directly via MediatR without making HTTP calls.

### API Mode
BlazorUI can also make HTTP calls to WebAPI using HttpClient.

---

## 🎓 Learning Path

```
1. QUICK_REFERENCE.md (5 min)
   ↓
2. ARCHITECTURE.md (15 min)
   ↓
3. BLAZOR_IMPLEMENTATION_SUMMARY.md (10 min)
   ↓
4. SHARED_VALIDATION_GUIDE.md (10 min)
   ↓
5. Individual project READMEs (as needed)
   ↓
6. Setup & Configuration docs (as needed)
```

Estimated total: **1-2 hours** to understand the full architecture

---

## 📝 Version Information

- **Framework**: .NET 8.0
- **Blazor**: Blazor Server
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 9.0
- **Pattern**: Clean Architecture + CQRS
- **Validation**: FluentValidation
- **Last Updated**: January 10, 2026
- **Build Status**: ✅ Passing

---

## 🤝 Contributing

When adding new features:
1. Follow the SOLID principles
2. Add validators to Shared project
3. Register validators in both WebAPI and BlazorUI
4. Update relevant documentation
5. Ensure the solution still builds

---

## 📞 Support

For questions or issues:
1. Check the relevant documentation file
2. Search for examples in existing code
3. Review the architecture guide
4. Check the quick reference

---

**Navigation Note**: Use this index to quickly find the documentation you need. Each file is focused on a specific aspect of the project for easy navigation and understanding.
