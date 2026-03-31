using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IExpenseRequestGetByIdUseCase
{
    Task<ExpenseRequestDto?> ExecuteAsync(Guid id);
}
