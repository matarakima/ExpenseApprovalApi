using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IRoleGetByIdUseCase
{
    Task<RoleDto?> ExecuteAsync(Guid id);
}
