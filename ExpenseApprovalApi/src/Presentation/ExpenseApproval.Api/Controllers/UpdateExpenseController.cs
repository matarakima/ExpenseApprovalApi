using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Expenses.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Updates an existing expense request.
/// </summary>
[ApiController]
[Route("api/expenses")]
[Authorize]
[Produces("application/json")]
public class UpdateExpenseController : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateExpenseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Updates an existing expense request.
    /// </summary>
    /// <param name="id">The unique identifier of the expense request to update.</param>
    /// <param name="dto">The updated expense request data.</param>
    /// <returns>The updated expense request.</returns>
    /// <response code="200">Expense request updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User lacks the required permission.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "expenses:edit")]
    [ProducesResponseType(typeof(ExpenseRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseRequestDto dto)
    {
        var command = new UpdateExpenseCommand(id, dto.CategoryId, dto.Description, dto.Amount, dto.ExpenseDate);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
