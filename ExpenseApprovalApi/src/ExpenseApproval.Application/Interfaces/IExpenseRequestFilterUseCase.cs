using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IExpenseRequestFilterUseCase
{
    Task<IEnumerable<ExpenseRequestDto>> ExecuteAsync(FilterExpenseRequestDto filter);
}
