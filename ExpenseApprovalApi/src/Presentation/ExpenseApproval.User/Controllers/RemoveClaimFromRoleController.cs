using ExpenseApproval.Application.Features.Roles.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

/// <summary>
/// Removes a permission claim from a role.
/// </summary>
[ApiController]
[Route("api/roles")]
[Authorize]
[Produces("application/json")]
public class RemoveClaimFromRoleController : ControllerBase
{
    private readonly IMediator _mediator;

    public RemoveClaimFromRoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Removes a permission claim from the specified role.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="claimValue">The claim value to remove from the role.</param>
    /// <response code="204">Claim removed successfully.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpDelete("{id:guid}/claims/{claimValue}")]
    [Authorize(Policy = "roles:remove-claim")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveClaim(Guid id, string claimValue)
    {
        await _mediator.Send(new RemoveClaimFromRoleCommand(id, claimValue));
        return NoContent();
    }
}
