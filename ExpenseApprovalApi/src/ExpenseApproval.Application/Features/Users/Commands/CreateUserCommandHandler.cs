using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Users.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserCreateUseCase _useCase;

    public CreateUserCommandHandler(IUserCreateUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = new CreateUserDto(request.Auth0Id, request.Email, request.FullName, request.RoleId);
        return await _useCase.ExecuteAsync(dto);
    }
}
