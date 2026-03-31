using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Retrieves all expense requests.
/// </summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
public class GetAllExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAllExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all expense requests.
    /// </summary>
    /// <returns>A list of expense requests.</returns>
    /// <response code="200">Returns the list of expenses.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpGet]
    [Authorize(Policy = "expenses:list")]
    [ProducesResponseType(typeof(IEnumerable<ExpenseRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllExpensesQuery());
        return Ok(result);
    }
}
