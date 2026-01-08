using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure.Abstractions;

namespace TaskManager.WebAPI.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, ITenantResolver tenantResolver, ITenantProvider tenantProvider)
    {
        var tenantId = await tenantResolver.ResolveTenantIdAsync(httpContext);
        if (tenantId.HasValue)
        {
            tenantProvider.SetTenant(tenantId.Value);
        }
        else
        {
            ///TODO no tenant found
        }
        await _next(httpContext);
    }
}