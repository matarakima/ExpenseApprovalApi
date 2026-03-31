using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Roles.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

/// <summary>
/// Adds a permission claim to a role.
/// </summary>
[ApiController]
[Route("api/roles")]
[Authorize]
[Produces("application/json")]
public class AddClaimToRoleController : ControllerBase
{
    private readonly IMediator _mediator;

    public AddClaimToRoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Adds a permission claim to the specified role.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="dto">The claim data containing the claim value to add.</param>
    /// <response code="204">Claim added successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpPost("{id:guid}/claims")]
    [Authorize(Policy = "roles:add-claim")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddClaim(Guid id, [FromBody] AddClaimDto dto)
    {
        await _mediator.Send(new AddClaimToRoleCommand(id, dto.ClaimValue));
        return NoContent();
    }
}
