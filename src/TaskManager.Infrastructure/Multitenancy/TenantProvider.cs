using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Multitenancy;

public class TenantProvider : ITenantProvider
{
    private Guid? _tenantId;
    public Guid? GetTenantId()
    {
        return _tenantId;
    }

    public Tenant? GetTenant()
    {
        throw new NotImplementedException();
    }

    public bool HasTenant()
    {
        return _tenantId.HasValue;
    }

    public void SetTenant(Guid? tenantId)
    {
         _tenantId = tenantId;
    }

    public void ClearTenant()
    {
        _tenantId = null;
    }
}