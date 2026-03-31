using ExpenseApproval.Application.DTOs;

namespace ExpenseApproval.Application.Interfaces;

public interface IUserGetByAuth0IdUseCase
{
    Task<UserDto?> ExecuteAsync(string auth0Id);
}
