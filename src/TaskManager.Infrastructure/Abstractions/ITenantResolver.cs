using Microsoft.AspNetCore.Http;

namespace TaskManager.Infrastructure.Abstractions;

public interface ITenantResolver
{
    Task<Guid?> ResolveTenantIdAsync(HttpContext httpContext);
}