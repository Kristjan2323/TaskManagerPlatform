using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Application.Features.Auth.DTOs;
using TaskManager.Application.Features.Auth.Services;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Presistence;

namespace TaskManager.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ITenantProvider _tenantProvider;
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ITenantProvider tenantProvider,
        ApplicationDbContext dbContext,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tenantProvider = tenantProvider;
        _dbContext = dbContext;
        _configuration = configuration;
    }
    public async Task<LoginResponseDto> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user is null)
        {
            throw new Exception("Invalid email or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        if (!result.Succeeded)
        {
            throw new Exception("Invalid email or password");
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate JWT token
        var token = GenerateJwtToken(user, roles);

        return new LoginResponseDto(
            token,
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            roles
        );
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        // Add tenant claim if user has a tenant
        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim("TenantId", user.TenantId.Value.ToString()));
        }

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<CreateUserResponseDto> RegisterUserAsync(CreateUserDto registerRequest)
    {
        var user = await _userManager.FindByEmailAsync(registerRequest.Email);
        if (user is not null) throw new Exception("Email already exists");

        var tenantId = _tenantProvider.GetTenantId();
        if (!tenantId.HasValue) throw new Exception("Tenant context is required for user registration");

        var userToCreate = new ApplicationUser
        {
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            TenantId = tenantId

        };

        var createUserResponse = await _userManager.CreateAsync(userToCreate, registerRequest.Password);
        return new CreateUserResponseDto(
            userToCreate.Id
            );
    }

    public async Task<CreateTenantResponseDto> CreateTenantAsync(CreateTenantDto createTenantDto)
    {
        // Check if tenant with same name exists
        var existingTenant = _dbContext.Tenants.FirstOrDefault(t => t.Name == createTenantDto.Name);
        if (existingTenant is not null)
        {
            throw new Exception("Tenant with this name already exists");
        }

        // Create the tenant
        var tenant = new Tenant
        {
            Name = createTenantDto.Name,
            isActive = true
        };

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();

        // Create the admin user for this tenant
        var adminUser = new ApplicationUser
        {
            Email = createTenantDto.AdminEmail,
            UserName = createTenantDto.AdminEmail,
            FirstName = createTenantDto.AdminFirstName,
            LastName = createTenantDto.AdminLastName,
            TenantId = tenant.Id,
            EmailConfirmed = true
        };

        var createUserResult = await _userManager.CreateAsync(adminUser, createTenantDto.AdminPassword);

        if (!createUserResult.Succeeded)
        {
            // Rollback tenant creation if user creation fails
            _dbContext.Tenants.Remove(tenant);
            await _dbContext.SaveChangesAsync();
            throw new Exception($"Failed to create admin user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
        }

        // Assign TenantAdmin role to the user
        await _userManager.AddToRoleAsync(adminUser, "TenantAdmin");

        return new CreateTenantResponseDto(
            tenant.Id,
            tenant.Name,
            adminUser.Id
        );
    }
}