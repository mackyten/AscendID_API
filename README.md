# AscendID API

[![CI](https://github.com/mackyten/AscendID_API/actions/workflows/ci.yml/badge.svg)](https://github.com/mackyten/AscendID_API/actions/workflows/ci.yml)

An identity provider built on ASP.NET Core Identity and [OpenIddict](https://documentation.openiddict.com/). It issues tokens through the OAuth 2.0 / OpenID Connect **authorization code flow with PKCE**, and also exposes a JSON API for registration, login, profile and token refresh. Clean Architecture, PostgreSQL, .NET 10.

## Features

- **OpenID Connect server**: authorize, token, userinfo and logout endpoints; authorization code and refresh token flows; scopes `openid`, `email`, `profile`, `roles`, `api`
- **Account API**: register, login, logout, profile read/update, change password, refresh token
- **JWT bearer authentication** for the account API (access + refresh tokens)
- **Role-based access** with ASP.NET Core Identity, plus a seeded admin user
- **Seeded OIDC clients**: `ascend-client` (confidential) and `ascend-spa` (public, PKCE required)
- **Clean Architecture**: Domain, Application (MediatR), Infrastructure (EF Core + Npgsql), WebAPI, plus a shared FluentValidation project and a Blazor UI
- **Swagger UI** in Development, automatic migrations and seeding on startup
- 30 xUnit tests, run in CI

## Quick start with Docker

```bash
cp .env.example .env        # then edit every value
docker compose up --build
```

The API listens on <http://localhost:8080> and Swagger is at <http://localhost:8080/swagger>. PostgreSQL runs in a container with a named volume. Migrations and the admin user are applied on first start.

## Run without Docker

You need the .NET 10 SDK and a PostgreSQL database.

```bash
cp src/AscendID_API.WebAPI/appsettings.json.example src/AscendID_API.WebAPI/appsettings.json
# edit the connection string, AdminUser and Jwt:Key (32+ characters)
dotnet run --project src/AscendID_API.WebAPI --launch-profile Local
```

The `Local` profile serves on <http://localhost:5120> with Swagger at `/swagger`; the examples below use port 8080 (Docker), so swap the port if you run it this way.

Environment-specific setup and the migration helper are described in [docs/ENVIRONMENT_SETUP.md](docs/ENVIRONMENT_SETUP.md).

## Configuration

| Setting | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `AdminUser:Email`, `AdminUser:Password` | Admin account seeded on startup |
| `Jwt:Key` | Signing key, at least 32 characters. The app refuses to issue tokens without it |
| `Jwt:Issuer`, `Jwt:Audience` | Token issuer and audience |

In Docker and CI these are environment variables with `__` as the separator (for example `Jwt__Key`).

## Endpoints

### OpenID Connect (`/connect`)

| Method | Path | Description |
|---|---|---|
| GET, POST | `/connect/authorize` | Start the authorization code flow |
| POST | `/connect/token` | Exchange a code or refresh token for tokens |
| GET, POST | `/connect/userinfo` | Claims for the access token's subject |
| GET, POST | `/connect/logout` | End the session |

### Account API (`/api/account`)

| Method | Path | Auth | Description |
|---|---|---|---|
| POST | `/api/account/register` | none | Create an account |
| POST | `/api/account/login` | none | Get an access and refresh token |
| POST | `/api/account/refresh-token` | none | Exchange a refresh token |
| POST | `/api/account/logout` | JWT | Sign out |
| GET | `/api/account/profile` | JWT | Current user's profile |
| PUT | `/api/account/profile` | JWT | Update first and last name |
| POST | `/api/account/change-password` | JWT | Change password |

Example:

```bash
curl -X POST http://localhost:8080/api/account/register \
  -H 'Content-Type: application/json' \
  -d '{"email":"ada@example.com","password":"Str0ng!Passw0rd","firstName":"Ada","lastName":"Lovelace"}'
# {"message":"Registration successful"}

curl -X POST http://localhost:8080/api/account/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"ada@example.com","password":"Str0ng!Passw0rd","rememberMe":false}'
# {"message":"Login successful","accessToken":"eyJ...","refreshToken":"...","expiresAt":"...","tokenType":"Bearer"}

curl http://localhost:8080/api/account/profile -H "Authorization: Bearer <accessToken>"
```

Registration auto-confirms the email, which is a demo-only shortcut.

### Demo CRUD (`/api/TempTestRecords`)

A small JWT-protected CRUD resource (`GET`, `GET {id}`, `POST`, `PUT {id}`, `DELETE {id}`) used to exercise the validation and MediatR pipeline. It is a scaffold, not part of the identity features.

## Authorization code flow with PKCE (the SPA client)

Register nothing; `ascend-spa` is seeded. Redirect users to `/connect/authorize` with `client_id=ascend-spa`, `response_type=code`, `redirect_uri=http://localhost:3000/callback`, a `code_challenge` (S256) and the scopes you need. Then `POST /connect/token` with `grant_type=authorization_code`, the `code`, and your `code_verifier`.

The seeded clients, including the confidential client's secret, are **development fixtures**. Change or remove them before deploying anywhere real (see `OpenIddictConfigurationService`).

## Project layout

```
src/
  AscendID_API.Domain/          entities (ApplicationUser, OAuth client and scope)
  AscendID_API.Application/     MediatR commands, queries, handlers, service interfaces
  AscendID_API.Infrastructure/  EF Core, OpenIddict setup, JWT and identity services
  AscendID_API.Shared/          FluentValidation rules shared by the API and the Blazor UI
  AscendID_API.WebAPI/          controllers, middleware, startup
  AscendID_API.BlazorUI/        demo UI
tests/AscendID_API.Tests/       xUnit + Moq
docs/                           architecture and setup notes
```

## Tests

```bash
dotnet test AscendID_API.sln
```

The suite covers the record entity, FluentValidation rules, MediatR handlers (against an in-memory repository) and `JwtTokenService`: issuing, validating, revoking and refreshing tokens, a wrong signing key, an inactive user, and the missing-key failure.

## More documentation

[docs/DOCUMENTATION_INDEX.md](docs/DOCUMENTATION_INDEX.md) links the architecture overview, identity implementation notes, environment setup and the JWT/Swagger guide.

## License

[MIT](LICENSE)
