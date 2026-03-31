using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Domain.Entities;

namespace ExpenseApproval.Application.UseCases;

public static class ExpenseRequestMapper
{
    public static ExpenseRequestDto MapToDto(ExpenseRequest e) => new(
        e.Id,
        e.Category?.Name ?? string.Empty,
        e.Description,
        e.Amount,
        e.ExpenseDate,
        e.RequestedBy?.FullName ?? string.Empty,
        e.Status.ToString(),
        e.CreatedAt,
        e.DecisionDate,
        e.DecisionBy?.FullName
    );
}
