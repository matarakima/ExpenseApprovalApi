using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Domain.Entities;

namespace ExpenseApproval.Application.UseCases;

public static class RoleMapper
{
    public static RoleDto MapToDto(AppRole r) => new(
        r.Id, r.Name,
        r.Claims?.Select(c => c.ClaimValue) ?? Enumerable.Empty<string>()
    );
}
