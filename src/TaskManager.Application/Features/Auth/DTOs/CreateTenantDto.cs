namespace TaskManager.Application.Features.Auth.DTOs;

public record CreateTenantDto(
    string Name,
    string AdminEmail,
    string AdminPassword,
    string AdminFirstName,
    string AdminLastName
);
