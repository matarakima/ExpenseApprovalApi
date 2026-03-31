using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IRoleGetAllUseCase
{
    Task<IEnumerable<RoleDto>> ExecuteAsync();
}
