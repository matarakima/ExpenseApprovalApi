using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IExpenseRequestApproveUseCase
{
    Task<ExpenseRequestDto> ExecuteAsync(Guid id, Guid decisionById);
}
