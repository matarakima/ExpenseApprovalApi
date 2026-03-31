using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

/// <summary>
/// Creates a new user.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
[Produces("application/json")]
public class CreateUserController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateUserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="command">The user creation data including Auth0Id, Email, FullName, and RoleId.</param>
    /// <returns>The newly created user.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpPost]
    [Authorize(Policy = "users:create")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Created($"/api/users/{result.Id}", result);
    }
}
