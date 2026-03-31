using Microsoft.AspNetCore.Authorization;

namespace ExpenseApproval.Infrastructure.Middlewares;

public class ClaimRequirement : IAuthorizationRequirement
{
    public string ClaimValue { get; }
    public ClaimRequirement(string claimValue) => ClaimValue = claimValue;
}

public class ClaimRequirementHandler : AuthorizationHandler<ClaimRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ClaimRequirement requirement)
    {
        if (context.User.HasClaim("permissions", requirement.ClaimValue))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
