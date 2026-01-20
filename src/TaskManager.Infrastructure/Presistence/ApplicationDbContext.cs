using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Presistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly ITenantProvider _tenantProvider;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        ITenantProvider tenantProvider): base(options)
    {
        _tenantProvider = tenantProvider;
    }
    
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Tenant itself should not be filtered by Id
        modelBuilder.Entity<Tenant>()
            .HasQueryFilter(t => true);

        // Other entities should be filtered by tenantId
        // modelBuilder.Entity<ApplicationUser>()
        //     .HasQueryFilter((u => u.TenantId == _tenantProvider.GetTenantId()));

         modelBuilder.Entity<ApplicationUser>()
        .HasOne(u => u.Tenant)
        .WithMany(t => t.Users)
        .HasForeignKey(u => u.TenantId)
        .OnDelete(DeleteBehavior.Restrict);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetTenantId();
        if(!tenantId.HasValue) throw new InvalidOperationException("Tenant not found");
        foreach (var entery in ChangeTracker.Entries<BaseEntity>())
        {
            if (entery.State == EntityState.Added)
            {
                if (entery.Entity is not Tenant)
                {
                    entery.Entity.TenantId = tenantId.Value;
                }

                entery.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entery.State == EntityState.Modified)
            {
                entery.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}