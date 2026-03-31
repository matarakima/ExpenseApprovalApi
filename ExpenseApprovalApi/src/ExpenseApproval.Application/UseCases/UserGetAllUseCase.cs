using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class UserGetAllUseCase : IUserGetAllUseCase
{
    private readonly IUserRepository _userRepository;

    public UserGetAllUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> ExecuteAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(UserMapper.MapToDto);
    }
}
