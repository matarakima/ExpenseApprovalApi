using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IExpenseRequestUpdateUseCase
{
    Task<ExpenseRequestDto> ExecuteAsync(Guid id, UpdateExpenseRequestDto dto);
}
