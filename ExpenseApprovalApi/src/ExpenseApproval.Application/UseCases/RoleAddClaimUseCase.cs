using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class RoleAddClaimUseCase : IRoleAddClaimUseCase
{
    private readonly IRoleRepository _roleRepository;

    public RoleAddClaimUseCase(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task ExecuteAsync(Guid roleId, string claimValue)
    {
        await _roleRepository.AddClaimAsync(roleId, claimValue);
    }
}
