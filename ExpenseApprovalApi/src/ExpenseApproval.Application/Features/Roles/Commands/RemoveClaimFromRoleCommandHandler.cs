using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Commands;

public class RemoveClaimFromRoleCommandHandler : IRequestHandler<RemoveClaimFromRoleCommand>
{
    private readonly IRoleRemoveClaimUseCase _useCase;

    public RemoveClaimFromRoleCommandHandler(IRoleRemoveClaimUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task Handle(RemoveClaimFromRoleCommand request, CancellationToken cancellationToken)
    {
        await _useCase.ExecuteAsync(request.RoleId, request.ClaimValue);
    }
}
