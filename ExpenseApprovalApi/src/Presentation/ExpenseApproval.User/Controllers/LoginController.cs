using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

/// <summary>
/// Handles user authentication via Auth0.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticates a user with email and password via Auth0.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <returns>An access token with token type and expiration.</returns>
    /// <response code="200">Authentication successful. Returns the JWT access token.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
