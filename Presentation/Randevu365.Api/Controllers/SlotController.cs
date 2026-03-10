using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Randevu365.Application.Features.BusinessSlots.Commands.RequestSlot;
using Randevu365.Application.Features.BusinessSlots.Commands.VerifyInAppPurchase;
using Randevu365.Application.Features.BusinessSlots.Queries.GetSlotPrice;
using Randevu365.Domain.Enum;

namespace Randevu365.Api.Controllers;

[Route("api/slot")]
[ApiController]
public class SlotController : ControllerBase
{
    private readonly IMediator _mediator;

    public SlotController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("price")]
    [Authorize(Roles = Roles.BusinessOwner)]
    public async Task<IActionResult> GetSlotPrice()
    {
        var response = await _mediator.Send(new GetSlotPriceQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("request")]
    [Authorize(Roles = Roles.BusinessOwner)]
    public async Task<IActionResult> RequestSlot(RequestSlotCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("verify-purchase")]
    [Authorize(Roles = Roles.BusinessOwner)]
    public async Task<IActionResult> VerifyInAppPurchase(VerifyInAppPurchaseCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}
