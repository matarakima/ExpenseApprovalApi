using ExpenseApproval.Application.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApproval.User.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize]
public class GetRoleByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetRoleByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "roles:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
