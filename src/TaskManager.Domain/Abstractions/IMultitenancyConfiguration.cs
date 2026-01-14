using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Abstractions;

public interface IMultitenancyConfiguration
{
   MultitenancyStrategy Strategy { get; }
   string TenantHeaderName { get; }
}