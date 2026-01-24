using TaskManager.Application.Features.Auth.DTOs;

namespace TaskManager.Application.Features.Auth.Services;

public interface IIdentityService
{
    public Task<LoginResponseDto> LoginAsync(LoginRequest loginRequest);
    public Task<CreateUserResponseDto> RegisterUserAsync (CreateUserDto registerRequest);
    public Task<CreateTenantResponseDto> CreateTenantAsync(CreateTenantDto createTenantDto);
}