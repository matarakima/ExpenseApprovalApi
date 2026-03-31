using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IRoleCreateUseCase
{
    Task<RoleDto> ExecuteAsync(CreateRoleDto dto);
}
