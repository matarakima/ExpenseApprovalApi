using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IUserCreateUseCase
{
    Task<UserDto> ExecuteAsync(CreateUserDto dto);
}
