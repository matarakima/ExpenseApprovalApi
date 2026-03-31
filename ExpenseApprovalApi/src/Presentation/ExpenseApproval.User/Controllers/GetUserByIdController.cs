using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

/// <summary>
/// Retrieves a single user by their identifier.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
[Produces("application/json")]
public class GetUserByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetUserByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user if found.</returns>
    /// <response code="200">Returns the user.</response>
    /// <response code="404">User not found.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "users:read")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
