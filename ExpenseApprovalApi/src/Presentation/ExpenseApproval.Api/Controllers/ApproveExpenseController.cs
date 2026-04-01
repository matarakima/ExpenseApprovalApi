using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Approves a pending expense request.
/// </summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
public class ApproveExpenseController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApproveExpenseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Approves a pending expense request by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the expense request to approve.</param>
    /// <param name="decisionById">The unique identifier of the user making the decision.</param>
    /// <returns>The approved expense request.</returns>
    /// <response code="200">Expense request approved successfully.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpPatch("{id:guid}/approve")]
    [Authorize(Policy = "expenses:approve")]
    [ProducesResponseType(typeof(ExpenseRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Approve(Guid id, [FromQuery] Guid decisionById)
    {
        var command = new ApproveExpenseCommand(id, decisionById);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
