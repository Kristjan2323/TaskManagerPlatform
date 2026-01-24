namespace TaskManager.Application.Features.Auth.DTOs;

public record CreateTenantResponseDto(
    Guid TenantId,
    string TenantName,
    Guid AdminUserId
);
