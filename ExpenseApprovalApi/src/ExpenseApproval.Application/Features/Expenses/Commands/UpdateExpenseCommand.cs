using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Commands;

public record UpdateExpenseCommand(Guid Id, Guid CategoryId, string Description, decimal Amount, DateTime ExpenseDate)
    : IRequest<ExpenseRequestDto>;
