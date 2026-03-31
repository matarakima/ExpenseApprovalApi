using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class UserGetByIdUseCase : IUserGetByIdUseCase
{
    private readonly IUserRepository _userRepository;

    public UserGetByIdUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> ExecuteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : UserMapper.MapToDto(user);
    }
}
