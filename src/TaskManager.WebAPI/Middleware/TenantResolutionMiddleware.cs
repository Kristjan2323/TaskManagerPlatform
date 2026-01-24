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
        // Check if user is SuperAdmin - they don't need a tenant

        {
            // SuperAdmin can operate without tenant context
            await _next(httpContext);
            return;
        }

        var tenantId = await tenantResolver.ResolveTenantIdAsync(httpContext);

        if (tenantId.HasValue)
        {
            tenantProvider.SetTenant(tenantId.Value);
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(new { error = "Tenant not found or not specified" });
            return;
        }
        await _next(httpContext);
    }
}