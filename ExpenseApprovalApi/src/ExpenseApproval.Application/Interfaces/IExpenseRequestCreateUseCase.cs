using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IExpenseRequestCreateUseCase
{
    Task<ExpenseRequestDto> ExecuteAsync(CreateExpenseRequestDto dto, Guid requestedById);
}
