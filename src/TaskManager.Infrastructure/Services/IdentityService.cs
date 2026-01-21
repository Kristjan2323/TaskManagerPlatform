using Microsoft.AspNetCore.Identity;
using TaskManager.Application.Features.Auth.DTOs;
using TaskManager.Application.Features.Auth.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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
}