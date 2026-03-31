using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Retrieves expense metrics and statistics.
/// </summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
public class GetExpenseMetricsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetExpenseMetricsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets aggregated expense metrics including counts and totals.
    /// </summary>
    /// <returns>Expense metrics with counts by status and total approved amount.</returns>
    /// <response code="200">Returns the expense metrics.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpGet("metrics")]
    [Authorize(Policy = "expenses:metrics")]
    [ProducesResponseType(typeof(ExpenseMetricsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMetrics()
    {
        var result = await _mediator.Send(new GetExpenseMetricsQuery());
        return Ok(result);
    }
}
