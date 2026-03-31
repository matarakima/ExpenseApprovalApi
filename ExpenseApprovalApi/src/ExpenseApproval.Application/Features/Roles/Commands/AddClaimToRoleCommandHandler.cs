using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Commands;

public class AddClaimToRoleCommandHandler : IRequestHandler<AddClaimToRoleCommand>
{
    private readonly IRoleAddClaimUseCase _useCase;

    public AddClaimToRoleCommandHandler(IRoleAddClaimUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task Handle(AddClaimToRoleCommand request, CancellationToken cancellationToken)
    {
        await _useCase.ExecuteAsync(request.RoleId, request.ClaimValue);
    }
}
