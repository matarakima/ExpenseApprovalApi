using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
