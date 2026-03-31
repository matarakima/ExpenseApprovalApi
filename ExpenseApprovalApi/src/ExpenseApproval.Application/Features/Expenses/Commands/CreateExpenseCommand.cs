using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Commands;

public record CreateExpenseCommand(Guid CategoryId, string Description, decimal Amount, DateTime ExpenseDate, Guid RequestedById)
    : IRequest<ExpenseRequestDto>;
