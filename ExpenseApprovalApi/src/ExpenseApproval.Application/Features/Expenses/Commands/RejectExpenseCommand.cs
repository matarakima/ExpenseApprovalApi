using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Commands;

public record RejectExpenseCommand(Guid Id, Guid DecisionById) : IRequest<ExpenseRequestDto>;
