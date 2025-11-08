# Environment Configuration Guide

## Connection String Configuration

This project supports multiple environments with different database configurations:

### Environment Mapping:
- **Local** → `appsettings.Local.json` → Local PostgreSQL database  
- **Development** → `appsettings.Development.json` → Supabase database
- **Production** → `appsettings.json` → Production database

## Database Migration Commands

### For Local Environment (using local PostgreSQL)
```bash
# Apply migrations
ASPNETCORE_ENVIRONMENT=Local dotnet ef database update --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI

# Add new migration  
ASPNETCORE_ENVIRONMENT=Local dotnet ef migrations add MigrationName --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI

# Drop database
ASPNETCORE_ENVIRONMENT=Local dotnet ef database drop --force --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI
```

### For Development Environment (using Supabase)
```bash
# Apply migrations
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI

# Add new migration
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add MigrationName --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI
```

### For Production Environment
```bash
# Apply migrations
ASPNETCORE_ENVIRONMENT=Production dotnet ef database update --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI
```

## Running the Application

### Using Launch Profiles (VS Code/Visual Studio)
1. **Local Profile**: Uses `appsettings.Local.json`
2. **Development Profile**: Uses `appsettings.Development.json`

### Using Command Line
```bash
# Run with Local environment
ASPNETCORE_ENVIRONMENT=Local dotnet run --project src/AscendID_API.WebAPI

# Run with Development environment  
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/AscendID_API.WebAPI

# Run with specific launch profile
dotnet run --project src/AscendID_API.WebAPI --launch-profile Local
dotnet run --project src/AscendID_API.WebAPI --launch-profile Development
```

## Configuration Files

### appsettings.json (Base/Production)
- Default connection string template
- Production admin user credentials
- Production-ready settings
- Fallback configuration

### appsettings.Local.json (Local Development)
- Local PostgreSQL database
- Database: `ascendid_api_identity`
- Port: 5433
- Local admin user: `admin@ascendid.local`

### appsettings.Development.json (Remote Development)
- Supabase PostgreSQL database
- Shared development database
- Port: 5432
- Development admin user: `admin@ascendid.dev`

## Important Notes

1. **Always specify the environment** when running EF commands to ensure you're working with the correct database
2. **Local environment** uses a local PostgreSQL instance with database name `ascendid_api_identity`
3. **Development environment** uses the shared Supabase database
4. **Migration files** are environment-agnostic but should be tested in all target environments
5. **Admin user credentials** are environment-specific and configured in each appsettings file
6. **Never commit sensitive passwords** - use environment variables or user secrets for production
7. **Use different admin emails** for each environment to avoid conflicts

## Quick Commands Reference

```bash
# Check which database will be used
ASPNETCORE_ENVIRONMENT=Local dotnet ef dbcontext info --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI

# View connection string being used
ASPNETCORE_ENVIRONMENT=Development dotnet ef dbcontext info --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI
```