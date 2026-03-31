using ExpenseApproval.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExpenseApproval.Infrastructure.Middlewares;

public class PermissionMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var auth0Id = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(auth0Id))
            {
                var user = await userRepository.GetByAuth0IdAsync(auth0Id);
                if (user?.Role?.Claims != null)
                {
                    var identity = context.User.Identity as ClaimsIdentity;
                    foreach (var claim in user.Role.Claims)
                    {
                        if (!context.User.HasClaim("permissions", claim.ClaimValue))
                        {
                            identity?.AddClaim(new Claim("permissions", claim.ClaimValue));
                        }
                    }
                }
            }
        }

        await _next(context);
    }
}
