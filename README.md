# AscendID API - Identity Provider with Clean Architecture

A comprehensive ASP.NET Core Identity Provider implementation using OpenIddict OIDC server, built with clean architecture principles.

## Features

- **ASP.NET Core Identity**: Complete user and role management
- **OpenIddict OIDC Provider**: OAuth 2.0/OpenID Connect server implementation
- **Clean Architecture**: Domain, Application, Infrastructure, and WebAPI layers
- **Entity Framework Core**: PostgreSQL with automatic migrations
- **Multi-Environment Support**: Local, Development, and Production configurations
- **RESTful API**: Complete account management endpoints
- **Swagger Documentation**: Interactive API documentation
- **Database Seeding**: Automatic role and admin user creation

## Prerequisites

1. **PostgreSQL Database**: Install PostgreSQL on your system
   - macOS: `brew install postgresql`
   - Ubuntu: `sudo apt install postgresql postgresql-contrib`
   - Windows: Download from [PostgreSQL official site](https://www.postgresql.org/download/)

2. **.NET 8 SDK**: Make sure you have .NET 8 SDK installed

## Quick Start

### Environment Configuration

This project supports multiple environments with different database configurations. See [ENVIRONMENT_SETUP.md](./ENVIRONMENT_SETUP.md) for detailed environment configuration and migration commands.

**Quick Environment Overview**:
- **Local** → `appsettings.Local.json` → Local PostgreSQL database  
- **Development** → `appsettings.Development.json` → Supabase database
- **Production** → `appsettings.json` → Production database

### Running with Different Environments

```bash
# Local development with local PostgreSQL
dotnet run --project src/AscendID_API.WebAPI --launch-profile Local

# Development with Supabase
dotnet run --project src/AscendID_API.WebAPI --launch-profile Development

# Using migration helper script
./migrate.sh update Local      # Update local database
./migrate.sh info Development  # Check development database info
```

### Setting up PostgreSQL

1. **Start PostgreSQL Service**:
   ```bash
   # macOS
   brew services start postgresql
   
   # Ubuntu
   sudo systemctl start postgresql
   
   # Windows
   # Use pgAdmin or start from Services
   ```

2. **Create Database and User**:
   ```sql
   -- Connect to PostgreSQL as superuser
   psql -U postgres
   
   -- Create development database
   CREATE DATABASE "AscendID_Dev_DB";
   
   -- Create production database (optional)
   CREATE DATABASE "AscendID_DB";
   
   -- Create user (if needed)
   CREATE USER ascendid_user WITH PASSWORD 'your_secure_password';
   GRANT ALL PRIVILEGES ON DATABASE "AscendID_Dev_DB" TO ascendid_user;
   ```

3. **Update Connection String** (if using custom user):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=AscendID_Dev_DB;Username=ascendid_user;Password=your_secure_password;Port=5432"
     }
   }
   ```

## Running the Application

1. **Navigate to WebAPI project**:
   ```bash
   cd src/AscendID_API.WebAPI
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

The application will:
- Connect to PostgreSQL
- Create the `temp_test_records` table automatically
- Start the API server (default: http://localhost:5120)

## Database Schema

The application creates the following table structure:

```sql
CREATE TABLE public.temp_test_records (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP,
    is_active BOOLEAN DEFAULT true NOT NULL
);

-- Indexes for better performance
CREATE INDEX ix_temp_test_records_is_active ON temp_test_records(is_active);
CREATE INDEX ix_temp_test_records_created_at ON temp_test_records(created_at);
```

## API Endpoints

Access the Swagger documentation at: `http://localhost:5120/swagger`

- `GET /api/TempTestRecords` - Get all records
- `GET /api/TempTestRecords/{id}` - Get record by ID
- `POST /api/TempTestRecords` - Create new record
- `PUT /api/TempTestRecords/{id}` - Update record
- `DELETE /api/TempTestRecords/{id}` - Soft delete record

## Troubleshooting

### Connection Issues

1. **Check PostgreSQL is running**:
   ```bash
   # Check if PostgreSQL is listening
   pg_isready -h localhost -p 5432
   ```

2. **Verify connection string**: Make sure the database name, username, and password are correct

3. **Check firewall settings**: Ensure port 5432 is accessible

### Common Errors

- **Database does not exist**: Create the database manually using `CREATE DATABASE`
- **Permission denied**: Grant proper permissions to the database user
- **Connection timeout**: Check if PostgreSQL service is running

## Environment Variables (Alternative)

You can also configure the connection string using environment variables:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Database=AscendID_Dev_DB;Username=postgres;Password=postgres;Port=5432"
```

Or create a `.env` file in the WebAPI project root:
```
ConnectionStrings__DefaultConnection=Host=localhost;Database=AscendID_Dev_DB;Username=postgres;Password=postgres;Port=5432
```

## Migration Commands (Future Use)

When you need to add database migrations:

```bash
# Add migration
dotnet ef migrations add InitialCreate --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI

# Update database
dotnet ef database update --project src/AscendID_API.Infrastructure --startup-project src/AscendID_API.WebAPI
```