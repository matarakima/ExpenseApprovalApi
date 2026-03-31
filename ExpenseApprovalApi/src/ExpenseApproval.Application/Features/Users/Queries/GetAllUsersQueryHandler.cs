using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Users.Queries;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserGetAllUseCase _useCase;

    public GetAllUsersQueryHandler(IUserGetAllUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync();
    }
}
