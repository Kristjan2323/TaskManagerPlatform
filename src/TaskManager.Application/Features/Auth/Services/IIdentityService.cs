using TaskManager.Application.Features.Auth.DTOs;

namespace TaskManager.Application.Features.Auth.Services;

public interface IIdentityService
{
    public UserDto LoginAsync(LoginRequest loginRequest);
    public Task<CreateUserResponseDto> RegisterUserAsync (CreateUserDto registerRequest);
}