FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so the layer is cached until a project file changes
COPY AscendID_API.sln ./
COPY src/AscendID_API.Domain/*.csproj src/AscendID_API.Domain/
COPY src/AscendID_API.Application/*.csproj src/AscendID_API.Application/
COPY src/AscendID_API.Infrastructure/*.csproj src/AscendID_API.Infrastructure/
COPY src/AscendID_API.Shared/*.csproj src/AscendID_API.Shared/
COPY src/AscendID_API.WebAPI/*.csproj src/AscendID_API.WebAPI/
COPY src/AscendID_API.BlazorUI/*.csproj src/AscendID_API.BlazorUI/
COPY tests/AscendID_API.Tests/*.csproj tests/AscendID_API.Tests/
RUN dotnet restore AscendID_API.sln

COPY . .
RUN dotnet publish src/AscendID_API.WebAPI -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AscendID_API.WebAPI.dll"]
