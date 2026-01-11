using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Randevu365.Application.Features.Businesses.Commands.CreateBusiness;
using Randevu365.Application.Features.Businesses.Commands.DeleteBusiness;
using Randevu365.Application.Features.Businesses.Commands.UpdateBusiness;
using Randevu365.Application.Features.Businesses.Queries.GetBusinessById;
using Randevu365.Application.Features.BusinessHours.Commands.CreateBusinessHour;
using Randevu365.Application.Features.BusinessHours.Commands.DeleteBusinessHour;
using Randevu365.Application.Features.BusinessHours.Commands.UpdateBusinessHour;
using Randevu365.Application.Features.BusinessHours.Queries.GetBusinessHoursByBusinessId;
using Randevu365.Application.Features.BusinessLocations.Commands.CreateBusinessLocation;
using Randevu365.Application.Features.BusinessLocations.Commands.DeleteBusinessLocation;
using Randevu365.Application.Features.BusinessLocations.Commands.UpdateBusinessLocation;
using Randevu365.Application.Features.BusinessLocations.Queries.GetBusinessLocationByBusinessId;
using Randevu365.Application.Features.BusinessLogo.Commands.CreateBusinessLogo;
using Randevu365.Application.Features.BusinessLogo.Commands.DeleteBusinessLogo;
using Randevu365.Application.Features.BusinessLogo.Commands.UpdateBusinessLogo;
using Randevu365.Application.Features.BusinessLogo.Queries.GetBusinessLogoByBusinessId;
using Randevu365.Application.Features.BusinessPhotos.Commands.CreateBusinessPhoto;
using Randevu365.Application.Features.BusinessPhotos.Commands.DeleteBusinessPhoto;
using Randevu365.Application.Features.BusinessPhotos.Queries.GetBusinessPhotosByBusinessId;
using Randevu365.Domain.Enum;

namespace Randevu365.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.BusinessOwner)]
public class BusinessController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Business Configuration
    [HttpGet("getbyid/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _mediator.Send(new GetBusinessByIdQueryRequest { Id = id });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateBusinessCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update(UpdateBusinessCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(DeleteBusinessCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
    #endregion

    #region Business Photos
    [HttpGet("getphotos/{businessId}")]
    public async Task<IActionResult> GetPhotos(int businessId)
    {
        var response = await _mediator.Send(new GetBusinessPhotosByBusinessIdQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("photo/create")]
    public async Task<IActionResult> CreatePhoto([FromForm] CreateBusinessPhotoCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("photo/delete")]
    public async Task<IActionResult> DeletePhoto(DeleteBusinessPhotoCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
    #endregion

    #region Business Locations
    [HttpGet("getlocations/{businessId}")]
    public async Task<IActionResult> GetLocations(int businessId)
    {
        var response = await _mediator.Send(new GetBusinessLocationByBusinessIdQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("location/create")]
    public async Task<IActionResult> CreateLocation(CreateBusinessLocationCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("location/update")]
    public async Task<IActionResult> UpdateLocation(UpdateBusinessLocationCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("location/delete")]
    public async Task<IActionResult> DeleteLocation(DeleteBusinessLocationCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
    #endregion

    #region Business Hours
    [HttpGet("gethours/{businessId}")]
    public async Task<IActionResult> GetHours(int businessId)
    {
        var response = await _mediator.Send(new GetBusinessHoursByBusinessIdQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("hour/create")]
    public async Task<IActionResult> CreateHour(CreateBusinessHourCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("hour/update")]
    public async Task<IActionResult> UpdateHour(UpdateBusinessHourCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("hour/delete")]
    public async Task<IActionResult> DeleteHour(DeleteBusinessHourCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
    #endregion

    #region Business Logo
    [HttpGet("getlogo/{businessId}")]
    public async Task<IActionResult> GetLogo(int businessId)
    {
        var response = await _mediator.Send(new GetBusinessLogoByBusinessIdQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("logo/create")]
    public async Task<IActionResult> CreateLogo([FromForm] CreateBusinessLogoCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("logo/update")]
    public async Task<IActionResult> UpdateLogo(UpdateBusinessLogoCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("logo/delete")]
    public async Task<IActionResult> DeleteLogo(DeleteBusinessLogoCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
    #endregion
}