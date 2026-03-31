using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Commands;

public record RemoveClaimFromRoleCommand(Guid RoleId, string ClaimValue) : IRequest;
