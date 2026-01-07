namespace TaskManager.Domain.Entities;

public enum MultitenancyStrategy
{
    SharedDatabase,
    DatabasePerTenant,
    Hybrid
}