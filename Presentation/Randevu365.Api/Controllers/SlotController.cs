using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Randevu365.Application.Features.BusinessSlots.Commands.ApprovePackage;
using Randevu365.Application.Features.BusinessSlots.Commands.ApproveSlot;
using Randevu365.Application.Features.BusinessSlots.Commands.RequestSlot;
using Randevu365.Application.Features.BusinessSlots.Queries.GetMySlots;
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

    [HttpGet("my")]
    [Authorize(Roles = Roles.BusinessOwner)]
    public async Task<IActionResult> GetMySlots()
    {
        var response = await _mediator.Send(new GetMySlotsQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> ApproveSlot(int id, ApproveSlotCommandRequest request)
    {
        request.SlotId = id;
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("package/{packageId}/approve")]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> ApprovePackage(Guid packageId, ApprovePackageCommandRequest request)
    {
        request.PackageId = packageId;
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}
