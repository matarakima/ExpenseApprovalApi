using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Queries;

public record GetAllExpensesQuery : IRequest<IEnumerable<ExpenseRequestDto>>;
