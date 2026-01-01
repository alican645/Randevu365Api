using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Features.AppUser.Queries.GetAllUser;
using Randevu365.Domain.Enum;

namespace Randevu365.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Administrator)]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<IList<GetAllAppUserQueryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IList<GetAllAppUserQueryResponse>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<IList<GetAllAppUserQueryResponse>>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _mediator.Send(new GetAllAppUserQueryRequest());
        return StatusCode(response.StatusCode, response);
    }
}