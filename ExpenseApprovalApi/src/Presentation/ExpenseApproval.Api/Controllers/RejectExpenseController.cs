using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Rejects a pending expense request.
/// </summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
public class RejectExpenseController : ControllerBase
{
    private readonly IMediator _mediator;

    public RejectExpenseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Rejects a pending expense request by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the expense request to reject.</param>
    /// <returns>The rejected expense request.</returns>
    /// <response code="200">Expense request rejected successfully.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpPatch("{id:guid}/reject")]
    [Authorize(Policy = "expenses:reject")]
    [ProducesResponseType(typeof(ExpenseRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Reject(Guid id)
    {
        // TODO: resolve AppUser Id from auth0 sub
        var command = new RejectExpenseCommand(id, Guid.Empty);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
