using AscendID_API.BlazorUI.Components;
using AscendID_API.BlazorUI.Services;
using AscendID_API.Application.Interfaces;
using AscendID_API.Infrastructure.Repositories;
using AscendID_API.Infrastructure;
using AscendID_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using AscendID_API.Shared.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for calling the WebAPI (if using API mode)
builder.Services.AddHttpClient("AscendIDAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// For Blazor Server mode with direct Application access
// Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<ITempTestRecordRepository, TempTestRecordRepository>();

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(AscendID_API.Application.DTOs.TempTestRecordDto).Assembly);
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateTempTestRecordDtoValidator>();

// Add custom services
builder.Services.AddScoped<TempTestRecordService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
