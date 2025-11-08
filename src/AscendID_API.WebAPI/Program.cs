using AscendID_API.WebAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure application services
builder.Services.ConfigureApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure middleware pipeline
app.ConfigureMiddleware();

// Configure database (migrations and seeding)
await app.ConfigureDatabaseAsync();

app.Run();
