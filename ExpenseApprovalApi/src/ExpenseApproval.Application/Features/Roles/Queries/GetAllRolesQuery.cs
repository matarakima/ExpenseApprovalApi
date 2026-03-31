using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Queries;

public record GetAllRolesQuery : IRequest<IEnumerable<RoleDto>>;
