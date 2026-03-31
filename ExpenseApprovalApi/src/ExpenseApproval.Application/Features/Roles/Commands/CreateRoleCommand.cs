using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Commands;

public record CreateRoleCommand(string Name) : IRequest<RoleDto>;
