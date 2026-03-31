using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Queries;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto?>
{
    private readonly IRoleGetByIdUseCase _useCase;

    public GetRoleByIdQueryHandler(IRoleGetByIdUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync(request.Id);
    }
}
