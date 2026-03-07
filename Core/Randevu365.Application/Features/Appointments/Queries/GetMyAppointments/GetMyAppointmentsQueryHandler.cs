using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetMyAppointments;

public class GetMyAppointmentsQueryHandler : IRequestHandler<GetMyAppointmentsQueryRequest, ApiResponse<List<GetMyAppointmentsQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyAppointmentsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<List<GetMyAppointmentsQueryResponse>>> Handle(GetMyAppointmentsQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<List<GetMyAppointmentsQueryResponse>>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var userId = _currentUserService.UserId.Value;

        var appointments = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAllAsync(
                predicate: a => a.AppUserId == userId && !a.IsDeleted
                    && (!request.OnlyActive.HasValue || !request.OnlyActive.Value
                        || (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed)),
                include: q => q.Include(a => a.Business!).Include(a => a.BusinessService!),
                orderBy: q => q.OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.RequestedStartTime));

        var response = appointments.Select(a => new GetMyAppointmentsQueryResponse
        {
            Id = a.Id,
            BusinessName = a.Business?.BusinessName ?? string.Empty,
            ServiceTitle = a.BusinessService?.ServiceTitle ?? string.Empty,
            AppointmentDate = a.AppointmentDate,
            RequestedStartTime = a.RequestedStartTime,
            RequestedEndTime = a.RequestedEndTime,
            ApproveStartTime = a.ApproveStartTime,
            ApproveEndTime = a.ApproveEndTime,
            Status = a.Status,
            CustomerNotes = a.CustomerNotes,
            BusinessNotes = a.BusinessNotes,
            CreatedAt = a.CreatedAt
        }).ToList();

        return ApiResponse<List<GetMyAppointmentsQueryResponse>>.SuccessResult(response);
    }
}
