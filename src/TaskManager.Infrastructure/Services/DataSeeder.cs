using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Services;

public class DataSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;

    public DataSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        // Create roles
        await CreateRoleIfNotExists("SuperAdmin");
        await CreateRoleIfNotExists("TenantAdmin");
        await CreateRoleIfNotExists("User");

        // Create SuperAdmin user
        await CreateSuperAdminIfNotExists();
    }

    private async Task CreateRoleIfNotExists(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    private async Task CreateSuperAdminIfNotExists()
    {
        var superAdminEmail = _configuration["SuperAdmin:Email"] ?? "superadmin@system.com";
        var superAdminPassword = _configuration["SuperAdmin:Password"] ?? "SuperAdmin123!";

        var superAdmin = await _userManager.FindByEmailAsync(superAdminEmail);

        if (superAdmin is null)
        {
            superAdmin = new ApplicationUser
            {
                Email = superAdminEmail,
                UserName = superAdminEmail,
                EmailConfirmed = true,
                FirstName = "Super",
                LastName = "Admin",
                TenantId = null // System user, no tenant
            };

            var result = await _userManager.CreateAsync(superAdmin, superAdminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }
        }
    }
}
