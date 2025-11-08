# AscendID API - OIDC Identity Provider Implementation

## Overview

This project has been successfully transformed into a comprehensive ASP.NET Core Identity Provider using OpenIddict for OIDC (OpenID Connect) support. The implementation follows clean architecture principles with clear separation of concerns.

## Architecture

### Domain Layer (`AscendID_API.Domain`)
- **ApplicationUser**: Extended IdentityUser with custom properties (first name, last name, profile picture, etc.)
- **ApplicationRole**: Extended IdentityRole with custom properties (description, system role flag, etc.)
- **OAuthClient**: Entity for managing OAuth client applications
- **OAuthScope**: Entity for managing OAuth scopes

### Application Layer (`AscendID_API.Application`)
- **IUserService**: Interface for user management operations
- **IRoleService**: Interface for role management operations
- **IAuthenticationService**: Interface for authentication operations
- **IOAuthClientService**: Interface for OAuth client management

### Infrastructure Layer (`AscendID_API.Infrastructure`)
- **ApplicationDbContext**: Extended to support Identity and OpenIddict
- Service implementations for all application interfaces
- **OpenIddictConfigurationService**: Background service for configuring OIDC clients and scopes
- Database configuration and seeding

### WebAPI Layer (`AscendID_API.WebAPI`)
- **AccountController**: REST API endpoints for account management
- **AuthorizationController**: OIDC authorization endpoints
- **AuthController**: MVC controller for authentication views
- Configuration for services, middleware, and database

## Features Implemented

### ✅ ASP.NET Core Identity Integration
- Custom user and role entities with extended properties
- PostgreSQL database support with proper table naming (snake_case)
- Password policies and account lockout configuration
- Role-based authorization

### ✅ OpenIddict OIDC Provider
- Authorization Code flow with PKCE support
- Refresh token support
- Standard OIDC endpoints:
  - `/connect/authorize` - Authorization endpoint
  - `/connect/token` - Token endpoint
  - `/connect/userinfo` - User info endpoint
  - `/connect/logout` - Logout endpoint
- Configurable OAuth clients and scopes
- Development certificates for signing and encryption

### ✅ Clean Architecture Compliance
- Clear separation between Domain, Application, Infrastructure, and WebAPI layers
- Dependency injection for all services
- Interface-based service contracts
- Repository pattern for data access

### ✅ Database Configuration
- Automatic migrations on startup
- Database seeding with default admin user and roles
- PostgreSQL optimized configuration with indexes

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL server running
- Connection string configured in `appsettings.json`

### Default Admin Credentials
```
Email: admin@ascendid.local
Password: AdminPassword123!
```

### Running the Application
```bash
cd /Users/angelo/AscendID_API
dotnet run --project src/AscendID_API.WebAPI
```

The application will:
1. Apply database migrations automatically
2. Seed initial roles and admin user
3. Configure OpenIddict clients and scopes
4. Start the OIDC provider on HTTPS

## API Endpoints

### Account Management
- `POST /api/account/login` - User login
- `POST /api/account/logout` - User logout  
- `POST /api/account/register` - User registration
- `GET /api/account/profile` - Get user profile
- `PUT /api/account/profile` - Update user profile
- `POST /api/account/change-password` - Change password

### OIDC Endpoints
- `GET/POST /connect/authorize` - Authorization endpoint
- `POST /connect/token` - Token endpoint
- `GET/POST /connect/userinfo` - User information endpoint
- `GET/POST /connect/logout` - Logout endpoint

## OAuth Clients

### Pre-configured Clients

#### 1. Confidential Client
- **Client ID**: `ascend-client`
- **Client Secret**: `ascend-client-secret-change-in-production`
- **Grant Types**: Authorization Code, Refresh Token
- **Scopes**: openid, email, profile, roles, api

#### 2. Public Client (SPA)
- **Client ID**: `ascend-spa`  
- **Grant Types**: Authorization Code with PKCE, Refresh Token
- **Scopes**: openid, email, profile, roles, api

## Database Schema

The implementation includes the following table groups:

### Identity Tables
- `users` - Application users
- `roles` - Application roles  
- `user_roles` - User role assignments
- `user_claims` - User claims
- `user_logins` - External login providers
- `user_tokens` - User tokens
- `role_claims` - Role claims

### OpenIddict Tables
- `OpenIddictApplications` - OAuth client applications
- `OpenIddictScopes` - OAuth scopes
- `OpenIddictAuthorizations` - User authorizations
- `OpenIddictTokens` - Access/refresh tokens

### Custom Tables
- `oauth_clients` - Custom OAuth client configuration
- `oauth_scopes` - Custom OAuth scope configuration
- `temp_test_records` - Original test entity (preserved)

## Security Considerations

### Development vs Production

**⚠️ IMPORTANT**: This implementation uses development certificates and default secrets. For production:

1. **Replace development certificates** with proper SSL certificates
2. **Change default admin password** and client secrets
3. **Configure proper CORS policies** for your domains
4. **Enable additional security features** like:
   - Rate limiting
   - IP whitelisting
   - Advanced logging
   - Health checks

### Password Policy
- Minimum 8 characters
- Requires uppercase, lowercase, digit, and special character
- Account lockout after 5 failed attempts for 5 minutes

## Next Steps

### Recommended Enhancements
1. **Add Two-Factor Authentication (2FA)**
2. **Implement external login providers** (Google, Microsoft, etc.)
3. **Add user management UI** for administrators
4. **Implement consent screens** for OAuth flows
5. **Add comprehensive logging and monitoring**
6. **Create client application management interface**
7. **Add API rate limiting and throttling**
8. **Implement advanced security headers**

### Client Integration Examples
The OIDC provider can be integrated with:
- ASP.NET Core MVC applications
- Blazor applications
- React/Angular/Vue.js SPAs
- Mobile applications
- Third-party applications supporting OIDC

## Testing the Implementation

### Using Swagger
1. Navigate to `/swagger` when running in development
2. Use the account endpoints to register and login
3. Test API endpoints with bearer tokens

### OIDC Discovery
Access the OIDC discovery document at:
```
https://localhost:5001/.well-known/openid_configuration
```

This implementation provides a solid foundation for an enterprise-ready identity provider while maintaining clean architecture principles and following security best practices.