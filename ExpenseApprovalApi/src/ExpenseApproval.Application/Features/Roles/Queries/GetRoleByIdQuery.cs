using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Queries;

public record GetRoleByIdQuery(Guid Id) : IRequest<RoleDto?>;
