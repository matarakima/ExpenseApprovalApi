using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

/// <summary>
/// Retrieves all available roles.
/// </summary>
[ApiController]
[Route("api/roles")]
[Authorize]
[Produces("application/json")]
public class GetAllRolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAllRolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all available roles with their associated claims.
    /// </summary>
    /// <returns>A list of all roles.</returns>
    /// <response code="200">Returns the list of roles.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpGet]
    [Authorize(Policy = "roles:list")]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllRolesQuery());
        return Ok(result);
    }
}
