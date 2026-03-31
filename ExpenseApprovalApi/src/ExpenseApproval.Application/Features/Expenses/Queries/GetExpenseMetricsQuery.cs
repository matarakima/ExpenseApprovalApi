using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Queries;

public record GetExpenseMetricsQuery : IRequest<ExpenseMetricsDto>;
