using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class UserGetByAuth0IdUseCase : IUserGetByAuth0IdUseCase
{
    private readonly IUserRepository _userRepository;

    public UserGetByAuth0IdUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> ExecuteAsync(string auth0Id)
    {
        var user = await _userRepository.GetByAuth0IdAsync(auth0Id);
        return user is null ? null : UserMapper.MapToDto(user);
    }
}
