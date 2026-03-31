using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Roles.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

/// <summary>
/// Creates a new role.
/// </summary>
[ApiController]
[Route("api/roles")]
[Authorize]
[Produces("application/json")]
public class CreateRoleController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateRoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new role in the system.
    /// </summary>
    /// <param name="command">The role creation data including the role name.</param>
    /// <returns>The newly created role.</returns>
    /// <response code="201">Role created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpPost]
    [Authorize(Policy = "roles:create")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return Created($"/api/roles/{result.Id}", result);
    }
}
