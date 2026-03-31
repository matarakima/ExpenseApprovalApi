using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Retrieves a single expense request by its identifier.
/// </summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
public class GetExpenseByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetExpenseByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets an expense request by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the expense request.</param>
    /// <returns>The expense request if found.</returns>
    /// <response code="200">Returns the expense request.</response>
    /// <response code="404">Expense request not found.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "expenses:read")]
    [ProducesResponseType(typeof(ExpenseRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetExpenseByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
