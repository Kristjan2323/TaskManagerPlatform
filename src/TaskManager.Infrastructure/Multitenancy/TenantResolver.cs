using Microsoft.AspNetCore.Http;
using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure.Abstractions;

namespace TaskManager.Infrastructure.Multitenancy;

public class TenantResolver: ITenantResolver
{
    private readonly IMultitenancyConfiguration _configuration;

    public TenantResolver(IMultitenancyConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public Task<Guid?> ResolveTenantIdAsync(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue(_configuration.TenantHeaderName, out var tenantIdValue))
        {
            if (Guid.TryParse(tenantIdValue, out var tenantId))
            {
                return Task.FromResult<Guid?>(tenantId);
            }
        }

        return Task.FromResult<Guid?>(null);
    }
}