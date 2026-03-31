using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class RoleRemoveClaimUseCase : IRoleRemoveClaimUseCase
{
    private readonly IRoleRepository _roleRepository;

    public RoleRemoveClaimUseCase(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task ExecuteAsync(Guid roleId, string claimValue)
    {
        await _roleRepository.RemoveClaimAsync(roleId, claimValue);
    }
}
