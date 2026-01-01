using MediatR;
using Microsoft.AspNetCore.Mvc;
using Randevu365.Application.Features.Businesses.Commands.CreateBusiness;
using Randevu365.Application.Features.Businesses.Commands.DeleteBusiness;
using Randevu365.Application.Features.Businesses.Commands.UpdateBusiness;
using Randevu365.Application.Features.Businesses.Queries.GetAllBusinesses;
using Randevu365.Application.Features.Businesses.Queries.GetBusinessById;
using Randevu365.Application.Features.BusinessLocations.Commands.CreateBusinessLocation;
using Randevu365.Application.Features.BusinessLocations.Commands.DeleteBusinessLocation;
using Randevu365.Application.Features.BusinessLocations.Commands.UpdateBusinessLocation;
using Randevu365.Application.Features.BusinessLocations.Queries.GetBusinessLocationByBusinessId;
using Randevu365.Application.Features.BusinessPhotos.Commands.CreateBusinessPhoto;
using Randevu365.Application.Features.BusinessPhotos.Commands.DeleteBusinessPhoto;
using Randevu365.Application.Features.BusinessPhotos.Queries.GetBusinessPhotosByBusinessId;

namespace Randevu365.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BusinessController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _mediator.Send(new GetAllBusinessesQueryRequest());
        return Ok(response);
    }

    [HttpGet("getbyid/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _mediator.Send(new GetBusinessByIdQueryRequest { Id = id });
        return Ok(response);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateBusinessCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update(UpdateBusinessCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(DeleteBusinessCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    // Business Photos

    [HttpGet("getphotos/{businessId}")]
    public async Task<IActionResult> GetPhotos(int businessId)
    {
        var response = await _mediator.Send(new GetBusinessPhotosByBusinessIdQueryRequest { BusinessId = businessId });
        return Ok(response);
    }

    [HttpPost("photo/create")]
    public async Task<IActionResult> CreatePhoto(CreateBusinessPhotoCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("photo/delete")]
    public async Task<IActionResult> DeletePhoto(DeleteBusinessPhotoCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    // Business Locations

    [HttpGet("getlocations/{businessId}")]
    public async Task<IActionResult> GetLocations(int businessId)
    {
        var response = await _mediator.Send(new GetBusinessLocationByBusinessIdQueryRequest { BusinessId = businessId });
        return Ok(response);
    }

    [HttpPost("location/create")]
    public async Task<IActionResult> CreateLocation(CreateBusinessLocationCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("location/update")]
    public async Task<IActionResult> UpdateLocation(UpdateBusinessLocationCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("location/delete")]
    public async Task<IActionResult> DeleteLocation(DeleteBusinessLocationCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}