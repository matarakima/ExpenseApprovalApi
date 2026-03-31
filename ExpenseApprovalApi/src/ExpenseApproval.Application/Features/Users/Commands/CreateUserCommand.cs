using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Users.Commands;

public record CreateUserCommand(string Auth0Id, string Email, string FullName, Guid RoleId) : IRequest<UserDto>;
