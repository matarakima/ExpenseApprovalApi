using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Filters expense requests by various criteria.
/// </summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
public class FilterExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilterExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Filters expense requests by status, category, and/or date range.
    /// </summary>
    /// <param name="status">Filter by expense status (e.g., Pending, Approved, Rejected).</param>
    /// <param name="category">Filter by expense category name.</param>
    /// <param name="fromDate">Start date for the date range filter.</param>
    /// <param name="toDate">End date for the date range filter.</param>
    /// <returns>A filtered list of expense requests.</returns>
    /// <response code="200">Returns the filtered list of expenses.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpGet("filter")]
    [Authorize(Policy = "expenses:filter")]
    [ProducesResponseType(typeof(IEnumerable<ExpenseRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Filter(
        [FromQuery] string? status,
        [FromQuery] string? category,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var query = new FilterExpensesQuery(status, category, fromDate, toDate);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
