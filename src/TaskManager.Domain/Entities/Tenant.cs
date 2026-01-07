namespace TaskManager.Domain.Entities;

public class Tenant : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public bool isActive { get; set; }
}