# Configuration Setup

This project uses appsettings.json files that contain sensitive information and are excluded from version control.

## Required Configuration Files

You need to create these files locally (they are ignored by git):

### 1. `appsettings.json`
Copy from `appsettings.json.example` and update with your production settings.

### 2. `appsettings.Development.json` 
Copy from `appsettings.Development.json.example` and update with your development settings.

## Setup Instructions

1. **Copy the example files:**
   ```bash
   cd src/AscendID_API.WebAPI
   cp appsettings.json.example appsettings.json
   cp appsettings.Development.json.example appsettings.Development.json
   ```

2. **Update the connection strings:**
   - Replace `your_username` with your PostgreSQL username
   - Replace `your_password` with your PostgreSQL password  
   - Replace `YourDatabaseName` with your actual database name

## Database Options

### Local PostgreSQL
```json
"DefaultConnection": "Host=localhost;Database=AscendID_DB;Username=postgres;Password=yourpassword;Port=5432"
```

### Supabase PostgreSQL
```json
"DefaultConnection": "Host=db.your-project-ref.supabase.co;Database=postgres;Username=postgres;Password=yourpassword;Port=5432;SSL Mode=Require"
```

### Docker PostgreSQL
```json
"DefaultConnection": "Host=localhost;Database=AscendID_DB;Username=postgres;Password=postgres;Port=5432"
```

## Security Notes

- ⚠️ **Never commit actual connection strings** to version control
- ✅ **Use environment variables** for production deployments
- ✅ **Keep sensitive data** in these local config files only
- ✅ **Example files are safe** to commit as they contain no real credentials

## Environment Variables (Production)

For production, consider using environment variables:
```bash
export ConnectionStrings__DefaultConnection="your-production-connection-string"
```