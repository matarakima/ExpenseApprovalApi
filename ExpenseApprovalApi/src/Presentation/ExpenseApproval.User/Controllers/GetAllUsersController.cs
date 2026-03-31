using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

/// <summary>
/// Retrieves all registered users.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
[Produces("application/json")]
public class GetAllUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAllUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all registered users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpGet]
    [Authorize(Policy = "users:list")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }
}
