using Scalar.AspNetCore;
using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure.Abstractions;
using TaskManager.Infrastructure.Multitenancy;
using TaskManager.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.MapGet("/tenant/current", (ITenantProvider tenantProvider) =>
    {
        var tenantId = tenantProvider.GetTenantId();
    
        if (tenantId.HasValue)
        {
            return Results.Ok(new { TenantId = tenantId.Value, Message = "Tenant resolved successfully" });
        }
    
        return Results.BadRequest(new { Message = "No tenant found in request" });
    })
    .WithName("GetCurrentTenant");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}