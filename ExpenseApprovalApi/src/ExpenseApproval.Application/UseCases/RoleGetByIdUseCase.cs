using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class RoleGetByIdUseCase : IRoleGetByIdUseCase
{
    private readonly IRoleRepository _roleRepository;

    public RoleGetByIdUseCase(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleDto?> ExecuteAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return role is null ? null : RoleMapper.MapToDto(role);
    }
}
