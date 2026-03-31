using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IUserGetAllUseCase
{
    Task<IEnumerable<UserDto>> ExecuteAsync();
}
