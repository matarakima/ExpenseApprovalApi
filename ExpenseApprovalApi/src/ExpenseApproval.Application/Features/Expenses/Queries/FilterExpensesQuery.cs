using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Queries;

public record FilterExpensesQuery(string? Status, string? Category, DateTime? FromDate, DateTime? ToDate)
    : IRequest<IEnumerable<ExpenseRequestDto>>;
