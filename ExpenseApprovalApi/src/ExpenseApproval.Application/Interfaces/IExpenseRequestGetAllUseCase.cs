using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IExpenseRequestGetAllUseCase
{
    Task<IEnumerable<ExpenseRequestDto>> ExecuteAsync();
}
