using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Queries;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IRoleGetAllUseCase _useCase;

    public GetAllRolesQueryHandler(IRoleGetAllUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync();
    }
}
