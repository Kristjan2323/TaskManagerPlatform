namespace TaskManager.Application.Features.Auth.DTOs;

public record CreateUserDto(
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyCollection<string> Roles,
    string? Password = null
    );