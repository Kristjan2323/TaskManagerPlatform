using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Abstractions;

public interface IMultitenancyConfiguration
{
   MultitenancyStrategy Strategy { get; }
   string TenantHeaderName { get; }
}