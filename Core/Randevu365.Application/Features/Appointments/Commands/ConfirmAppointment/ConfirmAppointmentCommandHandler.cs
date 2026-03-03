using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.ConfirmAppointment;

public class ConfirmAppointmentCommandHandler : IRequestHandler<ConfirmAppointmentCommandRequest, ApiResponse<ConfirmAppointmentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ConfirmAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<ConfirmAppointmentCommandResponse>> Handle(ConfirmAppointmentCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<ConfirmAppointmentCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var appointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(
                x => x.Id == request.AppointmentId && !x.IsDeleted,
                include: q => q.Include(a => a.Business).Include(a => a.BusinessService)
            );

        if (appointment == null)
        {
            return ApiResponse<ConfirmAppointmentCommandResponse>.NotFoundResult("Randevu bulunamadı.");
        }

        if (appointment.Business?.AppUserId != _currentUserService.UserId.Value)
        {
            return ApiResponse<ConfirmAppointmentCommandResponse>.ForbiddenResult("Bu randevuyu onaylama yetkiniz yok.");
        }

        if (appointment.Status != AppointmentStatus.Pending)
        {
            return ApiResponse<ConfirmAppointmentCommandResponse>.FailResult("Yalnızca beklemedeki randevular onaylanabilir.");
        }

        appointment.Status = AppointmentStatus.Confirmed;

        var confirmedStart = appointment.RequestedStartTime;
        var confirmedEnd = appointment.RequestedEndTime;

        if (confirmedStart != null && confirmedEnd != null && appointment.BusinessService != null)
        {
            var confirmedCount = await _unitOfWork.GetReadRepository<Appointment>()
                .CountAsync(x => x.BusinessServiceId == appointment.BusinessServiceId
                    && x.AppointmentDate == appointment.AppointmentDate
                    && x.Status == AppointmentStatus.Confirmed
                    && !x.IsDeleted
                    && x.RequestedStartTime < confirmedEnd
                    && x.RequestedEndTime > confirmedStart);

            if (confirmedCount >= appointment.BusinessService.MaxConcurrentCustomers)
            {
                var overlappingPending = await _unitOfWork.GetReadRepository<Appointment>()
                    .GetAllAsync(
                        x => x.BusinessServiceId == appointment.BusinessServiceId
                            && x.AppointmentDate == appointment.AppointmentDate
                            && x.Status == AppointmentStatus.Pending
                            && !x.IsDeleted
                            && x.Id != appointment.Id
                            && x.RequestedStartTime < confirmedEnd
                            && x.RequestedEndTime > confirmedStart,
                        enableTracking: true);

                foreach (var pending in overlappingPending)
                {
                    pending.Status = AppointmentStatus.Displaced;
                    await _unitOfWork.GetWriteRepository<Appointment>().UpdateAsync(pending);
                }
            }
        }

        await _unitOfWork.GetWriteRepository<Appointment>().UpdateAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<ConfirmAppointmentCommandResponse>.SuccessResult(
            new ConfirmAppointmentCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu başarıyla onaylandı.");
    }
}
