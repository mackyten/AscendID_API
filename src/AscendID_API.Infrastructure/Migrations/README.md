# Database Migrations

This folder contains Entity Framework Core migrations for the AscendID API project.

## Location
```
src/AscendID_API.Infrastructure/Migrations/
```

This follows Clean Architecture principles where the Infrastructure layer handles data persistence concerns.

## Current Migrations

### InitialCreate (20251108083100)
- Creates the `temp_test_records` table with PostgreSQL-specific configuration
- Uses snake_case naming convention for PostgreSQL
- Includes performance indexes on `created_at` and `is_active` columns
- Sets up default values for `created_at` (CURRENT_TIMESTAMP) and `is_active` (true)

## Table Schema
```sql
CREATE TABLE public.temp_test_records (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE,
    is_active BOOLEAN DEFAULT true
);

-- Indexes
CREATE INDEX ix_temp_test_records_created_at ON public.temp_test_records(created_at);
CREATE INDEX ix_temp_test_records_is_active ON public.temp_test_records(is_active);
```

## Commands

### Create a new migration
```bash
cd src/AscendID_API.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../AscendID_API.WebAPI
```

### Apply migrations
```bash
cd src/AscendID_API.Infrastructure  
dotnet ef database update --startup-project ../AscendID_API.WebAPI
```

### Remove last migration (if not applied)
```bash
cd src/AscendID_API.Infrastructure
dotnet ef migrations remove --startup-project ../AscendID_API.WebAPI
```

### Generate SQL script
```bash
cd src/AscendID_API.Infrastructure
dotnet ef migrations script --startup-project ../AscendID_API.WebAPI
```

## Configuration

The migrations use the connection string from:
- `appsettings.Development.json` (for Development environment)
- `appsettings.json` (for Production environment)

Current connection points to Supabase PostgreSQL database.

## Automatic Migration

The application automatically applies pending migrations on startup via `Program.cs`:
```csharp
context.Database.Migrate();
```

This is suitable for development but should be replaced with manual migration deployment for production.

## Best Practices

1. **Always backup production database** before applying migrations
2. **Test migrations on staging** environment first
3. **Review generated migration code** before applying
4. **Use descriptive migration names** that explain the changes
5. **Keep migrations small and focused** on single changes
6. **Never modify existing migration files** after they've been applied to production