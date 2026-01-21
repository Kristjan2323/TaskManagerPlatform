namespace TaskManager.Application.Features.Auth.DTOs;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName
    );