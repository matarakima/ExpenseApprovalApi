using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Queries;

public record GetExpenseByIdQuery(Guid Id) : IRequest<ExpenseRequestDto?>;
