using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IExpenseRequestGetMetricsUseCase
{
    Task<ExpenseMetricsDto> ExecuteAsync();
}
