using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Features.AppUser.Queries.GetAllUser;
using Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByBusinessId;
using Randevu365.Application.Features.Businesses.Queries.GetAllBusinesses;
using Randevu365.Domain.Enum;

namespace Randevu365.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = Roles.Administrator)]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region User Operations
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _mediator.Send(new GetAllAppUserQueryRequest());
        return StatusCode(response.StatusCode, response);
    }
    #endregion

    #region Business Operations
    [HttpGet("businesses")]
    public async Task<IActionResult> GetAllBusinesses()
    {
        var response = await _mediator.Send(new GetAllBusinessesQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("business/{businessId}/comments")]
    public async Task<IActionResult> GetCommentsByBusinessId(int businessId)
    {
        var response = await _mediator.Send(new GetCommentsByBusinessIdQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }
    #endregion
}