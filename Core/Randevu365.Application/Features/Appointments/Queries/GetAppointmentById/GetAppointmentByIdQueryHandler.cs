using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQueryRequest, ApiResponse<GetAppointmentByIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetAppointmentByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetAppointmentByIdQueryResponse>> Handle(GetAppointmentByIdQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<GetAppointmentByIdQueryResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var appointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(
                x => x.Id == request.AppointmentId && !x.IsDeleted,
                include: q => q.Include(a => a.Business).Include(a => a.BusinessService).Include(a => a.AppUser)
            );

        if (appointment == null)
        {
            return ApiResponse<GetAppointmentByIdQueryResponse>.NotFoundResult("Randevu bulunamadı.");
        }

        var currentUserId = _currentUserService.UserId.Value;
        var isCustomer = appointment.AppUserId == currentUserId;
        var isBusinessOwner = appointment.Business?.AppUserId == currentUserId;

        if (!isCustomer && !isBusinessOwner)
        {
            return ApiResponse<GetAppointmentByIdQueryResponse>.ForbiddenResult("Bu randevuya erişim yetkiniz yok.");
        }

        var response = new GetAppointmentByIdQueryResponse
        {
            Id = appointment.Id,
            BusinessName = appointment.Business?.BusinessName,
            ServiceTitle = appointment.BusinessService?.ServiceTitle,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status,
            CustomerNotes = appointment.CustomerNotes,
            BusinessNotes = appointment.BusinessNotes,
            AppUserId = appointment.AppUserId,
            BusinessId = appointment.BusinessId
        };

        return ApiResponse<GetAppointmentByIdQueryResponse>.SuccessResult(response);
    }
}
