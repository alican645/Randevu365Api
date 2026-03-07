using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommandRequest, ApiResponse<CancelAppointmentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CancelAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CancelAppointmentCommandResponse>> Handle(CancelAppointmentCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<CancelAppointmentCommandResponse>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var appointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(a => a.Id == request.AppointmentId && !a.IsDeleted, enableTracking: true);

        if (appointment == null)
            return ApiResponse<CancelAppointmentCommandResponse>.NotFoundResult("Randevu bulunamadi.");

        if (appointment.AppUserId != _currentUserService.UserId.Value)
            return ApiResponse<CancelAppointmentCommandResponse>.ForbiddenResult("Bu randevuyu iptal etme yetkiniz yok.");

        if (appointment.Status == AppointmentStatus.Completed)
            return ApiResponse<CancelAppointmentCommandResponse>.FailResult("Tamamlanmis bir randevu iptal edilemez.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            return ApiResponse<CancelAppointmentCommandResponse>.FailResult("Randevu zaten iptal edilmis.");

        appointment.Status = AppointmentStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(request.CancellationReason))
            appointment.CustomerNotes = request.CancellationReason;

        await _unitOfWork.GetWriteRepository<Appointment>().UpdateAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CancelAppointmentCommandResponse>.SuccessResult(
            new CancelAppointmentCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu basariyla iptal edildi.");
    }
}
