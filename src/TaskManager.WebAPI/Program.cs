using Scalar.AspNetCore;
using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure.Abstractions;
using TaskManager.Infrastructure.Multitenancy;
using TaskManager.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Register multitenancy services
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddSingleton<IMultitenancyConfiguration>(serviceProvider =>
{
    var configuration = new MultitenancyConfiguration
    {
        TenantHeaderName = builder.Configuration["Multitenancy:TenantHeaderName"] ?? "X-Tenant-Id"
    };
    return configuration;
});

// Add HttpContextAccessor (needed for accessing HttpContext in services)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Tenant Middleware
app.UseMiddleware<TenantResolutionMiddleware>();
app.MapControllers();

app.Run();
