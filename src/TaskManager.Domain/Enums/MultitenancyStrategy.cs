namespace TaskManager.Domain.Enums;

public enum MultitenancyStrategy
{
    SharedDatabase,
    DatabasePerTenant,
    Hybrid
}