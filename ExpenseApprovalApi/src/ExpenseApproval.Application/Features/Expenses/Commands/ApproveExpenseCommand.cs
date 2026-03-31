using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Commands;

public record ApproveExpenseCommand(Guid Id, Guid DecisionById) : IRequest<ExpenseRequestDto>;
