using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IUserGetByIdUseCase
{
    Task<UserDto?> ExecuteAsync(Guid id);
}
