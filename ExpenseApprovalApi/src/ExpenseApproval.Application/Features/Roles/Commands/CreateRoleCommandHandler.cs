using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Commands;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
{
    private readonly IRoleCreateUseCase _useCase;

    public CreateRoleCommandHandler(IRoleCreateUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var dto = new CreateRoleDto(request.Name);
        return await _useCase.ExecuteAsync(dto);
    }
}
