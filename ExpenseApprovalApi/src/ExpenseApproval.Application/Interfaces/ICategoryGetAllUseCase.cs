using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface ICategoryGetAllUseCase
{
    Task<IEnumerable<CategoryDto>> ExecuteAsync();
}
