using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class RoleGetAllUseCase : IRoleGetAllUseCase
{
    private readonly IRoleRepository _roleRepository;

    public RoleGetAllUseCase(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<RoleDto>> ExecuteAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(RoleMapper.MapToDto);
    }
}
