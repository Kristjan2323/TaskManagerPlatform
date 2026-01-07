using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Abstractions;

public interface ITenantProvider
{
    Guid? GetTenantId();
    Tenant? GetTenant();
    bool HasTenant();
    void SetTenant(Guid tenantId);
    void ClearTenant();
}