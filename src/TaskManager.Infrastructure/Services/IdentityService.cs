using Microsoft.AspNetCore.Identity;
using TaskManager.Application.Features.Auth.DTOs;
using TaskManager.Application.Features.Auth.Services;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantProvider _tenantProvider;

    public IdentityService(UserManager<ApplicationUser> userManager, ITenantProvider tenantProvider)
    {
        _userManager = userManager;
        _tenantProvider = tenantProvider;
    }
    public UserDto LoginAsync(LoginRequest loginRequest)
    {
        var user = _userManager.FindByEmailAsync(loginRequest.Email);
        if (user is null)
        {
            return default;
        }

        return new UserDto
        (
            user.Result.Id,
            user.Result.Email,
            user.Result.FirstName,
            user.Result.LastName,
            user.Result.UserName

        );
    }

    public async Task<CreateUserResponseDto> RegisterUserAsync(CreateUserDto registerRequest)
    {
        var user = await _userManager.FindByEmailAsync(registerRequest.Email);
        if (user is not null) throw new Exception("Email already exists");

        var userToCreate = new ApplicationUser
        {
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            TenantId = _tenantProvider.GetTenantId()!.Value

        };

        var createUserResponse = await _userManager.CreateAsync(userToCreate, registerRequest.Password);
        return new CreateUserResponseDto(
            userToCreate.Id
            );
    }
}