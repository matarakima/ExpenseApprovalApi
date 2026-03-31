using MediatR;

namespace ExpenseApproval.Application.Features.Roles.Commands;

public record AddClaimToRoleCommand(Guid RoleId, string ClaimValue) : IRequest;
