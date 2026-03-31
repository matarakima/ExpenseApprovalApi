using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IExpenseRequestRejectUseCase
{
    Task<ExpenseRequestDto> ExecuteAsync(Guid id, Guid decisionById);
}
