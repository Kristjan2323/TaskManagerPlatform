namespace TaskManager.Application.Features.Auth.DTOs;

public record LoginResponseDto(
    string Token,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    IEnumerable<string> Roles
);
