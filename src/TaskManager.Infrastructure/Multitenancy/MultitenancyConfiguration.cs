using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Multitenancy;

public class MultitenancyConfiguration : IMultitenancyConfiguration
{
    public MultitenancyStrategy Strategy { get; } = MultitenancyStrategy.SharedDatabase;
    public string TenantHeaderName { get; init; } = "X-Tenant-Id";
}