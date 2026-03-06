using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Randevu365.Application.Features.Appointments.Commands.CompleteAppointment;
using Randevu365.Application.Features.Businesses.Commands.CreateBusiness;
using Randevu365.Application.Features.Businesses.Commands.CreateBusinessDetail;
using Randevu365.Application.Features.Businesses.Commands.UpdateBusinessDetail;
using Randevu365.Application.Features.Businesses.Commands.UpdateBusinessDetailById;
using Randevu365.Application.Features.Businesses.Queries.GetBusinessByFilter;
using Randevu365.Application.Features.Businesses.Queries.GetBusinessesByBusinessCategory;
using Randevu365.Application.Features.Businesses.Queries.GetBusinessSummariesByOwner;
using Randevu365.Application.Features.Businesses.Queries.GetTopRatedBusinesses;
using Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessBasicInfoByCustomerOwnerId;
using Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByBusinessId;
using Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByCustomerOwnerId;
using Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessOwnerDashboard;
using Randevu365.Application.Features.BusinessProfile.Queries.GetRandomBusinessSummaryByOwner;
using Randevu365.Application.Features.Appointments.Commands.ConfirmAppointment;
using Randevu365.Application.Features.Appointments.Commands.RejectAppointment;
using Randevu365.Application.Features.Appointments.Commands.RevertConfirmedAppointment;
using Randevu365.Application.Features.Appointments.Queries.GetPendingAppointmentsByBusiness;
using Randevu365.Application.Features.Appointments.Queries.GetAllPendingAppointmentsByOwner;
using Randevu365.Application.Features.Appointments.Queries.GetConfirmedAppointmentsByBusiness;
using Randevu365.Application.Features.Appointments.Queries.GetBusinessBusySlotsByOwner;
using Randevu365.Application.Features.Appointments.Commands.CreateAppointmentByOwner;

using Randevu365.Domain.Enum;

namespace Randevu365.Api.Controllers;

[Route("api/business")]
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

    [AllowAnonymous]
    [HttpGet("filter")]
    public async Task<IActionResult> GetBusinessesByFilter(
        [FromQuery] string? category,
        [FromQuery] string? queryParam,
        [FromQuery] bool? orderByRating,
        [FromQuery] bool? orderByCommentCount,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await _mediator.Send(new GetBusinessByFilterRequest
        {
            Category = category,
            QueryParam = queryParam,
            OrderByRating = orderByRating,
            OrderByCommentCount = orderByCommentCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return StatusCode(response.StatusCode, response);
    }

    [AllowAnonymous]
    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRatedBusinesses([FromQuery] int count = 10)
    {
        var response = await _mediator.Send(new GetTopRatedBusinessesQueryRequest { Count = count });
        return StatusCode(response.StatusCode, response);
    }

    [AllowAnonymous]
    [HttpGet("by-category/{categoryName}")]
    public async Task<IActionResult> GetBusinessesByCategory(string categoryName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _mediator.Send(new GetBusinessesByBusinessCategoryRequest
        {
            CategoryName = categoryName,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("my-summaries")]
    public async Task<IActionResult> GetMySummaries()
    {
        var response = await _mediator.Send(new GetBusinessSummariesByOwnerQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("random-business-summary")]
    public async Task<IActionResult> GetRandomBusinessSummary()
    {
        var response = await _mediator.Send(new GetRandomBusinessSummaryByOwnerQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var response = await _mediator.Send(new GetBusinessOwnerDashboardQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("basicinfo")]
    public async Task<IActionResult> GetBusinessBasicInfo()
    {
        var response = await _mediator.Send(new GetBusinessBasicInfoByCustomerOwnerIdQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("detailinfo")]
    public async Task<IActionResult> GetBusinessDetailInfo()
    {
        var response = await _mediator.Send(new GetBusinessDetailInfoByCustomerOwnerIdQueryRequest());
        return StatusCode(response.StatusCode, response);
    }

    [AllowAnonymous]
    [HttpGet("{id}/detailinfo")]
    public async Task<IActionResult> GetDetailInfoById(int id)
    {
        var response = await _mediator.Send(new GetBusinessDetailInfoByBusinessIdQueryRequest { BusinessId = id });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateBusinessCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("detail/create")]
    public async Task<IActionResult> CreateDetail([FromForm] CreateBusinessDetailCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("detail/update")]
    public async Task<IActionResult> UpdateDetail([FromForm] UpdateBusinessDetailCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id}/detail/update")]
    public async Task<IActionResult> UpdateDetailById(int id, [FromForm] UpdateBusinessDetailByIdCommandRequest request)
    {
        request.BusinessId = id;
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
    #endregion

    #region Appointments

    [HttpGet("appointments/pending")]
    public async Task<IActionResult> GetAllPendingAppointments()
    {
        var response = await _mediator.Send(new GetAllPendingAppointmentsByOwnerQueryRequest());
        return StatusCode(response.StatusCode, response);
    }
    [HttpGet("appointments/confirmed")]
    public async Task<IActionResult> GetAllConfirmedAppointments([FromQuery] bool onlyConfirmed)
    {
        var response = await _mediator.Send(new GetAllConfirmedAppointmentsByOwnerQueryRequest { OnlyConfirmed = onlyConfirmed });
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("appointments/{businessId}/pending")]
    public async Task<IActionResult> GetPendingAppointments(int businessId)
    {
        var response = await _mediator.Send(new GetPendingAppointmentsByBusinessQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("appointments/{businessId}/{onlyConfirmed}/confirmed")]
    public async Task<IActionResult> GetConfirmedAppointments(int businessId,bool onlyConfirmed = false)
    {
        var response = await _mediator.Send(new GetConfirmedAppointmentsByBusinessQueryRequest { BusinessId = businessId, OnlyConfirmed = onlyConfirmed });
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("appointments/{businessId}/busy-slots")]
    public async Task<IActionResult> GetBusinessBusySlots(int businessId)
    {
        var response = await _mediator.Send(new GetBusinessBusySlotsByOwnerQueryRequest { BusinessId = businessId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("appointment/create-by-owner")]
    public async Task<IActionResult> CreateAppointmentByOwner([FromBody] CreateAppointmentByOwnerCommandRequest request)
    {
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("appointment/{appointmentId}/confirm")]
    public async Task<IActionResult> ConfirmAppointment(int appointmentId)
    {
        var response = await _mediator.Send(new ConfirmAppointmentCommandRequest { AppointmentId = appointmentId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("appointment/{appointmentId}/reject")]
    public async Task<IActionResult> RejectAppointment(int appointmentId, [FromBody] RejectAppointmentCommandRequest request)
    {
        request.AppointmentId = appointmentId;
        var response = await _mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("appointment/{appointmentId}/complete")]
    public async Task<IActionResult> CompleteAppointment(int appointmentId)
    {
        var response = await _mediator.Send(new CompleteAppointmentCommandRequest { AppointmentId = appointmentId });
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("appointment/{appointmentId}/revert-confirm")]
    public async Task<IActionResult> RevertConfirmedAppointment(int appointmentId)
    {
        var response = await _mediator.Send(new RevertConfirmedAppointmentCommandRequest { AppointmentId = appointmentId });
        return StatusCode(response.StatusCode, response);
    }
    #endregion
}
