using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Users.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserGetByIdUseCase _useCase;

    public GetUserByIdQueryHandler(IUserGetByIdUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync(request.Id);
    }
}
