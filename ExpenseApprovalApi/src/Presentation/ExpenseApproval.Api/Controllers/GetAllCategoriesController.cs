using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.Api.Controllers;

/// <summary>
/// Retrieves all categories.
/// </summary>
[ApiController]
[Route("api/categories")]
[Authorize]
[Produces("application/json")]
public class GetAllCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAllCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all categories.
    /// </summary>
    /// <returns>A list of categories.</returns>
    /// <response code="200">Returns the list of categories.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(result);
    }
}
