using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class RoleCreateUseCase : IRoleCreateUseCase
{
    private readonly IRoleRepository _roleRepository;

    public RoleCreateUseCase(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleDto> ExecuteAsync(CreateRoleDto dto)
    {
        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        var created = await _roleRepository.AddAsync(role);
        return RoleMapper.MapToDto(created);
    }
}
