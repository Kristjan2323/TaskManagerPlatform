using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Multitenancy;

public class MultitenancyConfiguration : IMultitenancyConfiguration
{
    public MultitenancyStrategy Strategy { get; } = MultitenancyStrategy.SharedDatabase;
    public string TenantHeaderName { get; init; } = "X-Tenant-Id";
}