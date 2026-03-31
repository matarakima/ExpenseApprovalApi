using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Domain.Entities;

namespace ExpenseApproval.Application.UseCases;

public static class UserMapper
{
    public static UserDto MapToDto(AppUser u) => new(
        u.Id, u.Auth0Id, u.Email, u.FullName,
        u.Role?.Name ?? string.Empty,
        u.Role?.Claims?.Select(c => c.ClaimValue) ?? Enumerable.Empty<string>()
    );
}
