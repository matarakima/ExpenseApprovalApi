using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Creates a new expense request.
/// </summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
public class CreateExpenseController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateExpenseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new expense request.
    /// </summary>
    /// <param name="dto">The expense request data.</param>
    /// <returns>The newly created expense request.</returns>
    /// <response code="201">Expense request created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpPost]
    [Authorize(Policy = "expenses:create")]
    [ProducesResponseType(typeof(ExpenseRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequestDto dto)
    {
        var command = new CreateExpenseCommand(dto.CategoryId, dto.Description, dto.Amount, dto.ExpenseDate, dto.RequestedById);
        var result = await _mediator.Send(command);
        return Created($"/api/expenses/{result.Id}", result);
    }
}
