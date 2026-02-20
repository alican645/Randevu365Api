using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointmentByBusiness;

public class CancelAppointmentByBusinessCommandHandler : IRequestHandler<CancelAppointmentByBusinessCommandRequest, ApiResponse<CancelAppointmentByBusinessCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CancelAppointmentByBusinessCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CancelAppointmentByBusinessCommandResponse>> Handle(CancelAppointmentByBusinessCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<CancelAppointmentByBusinessCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var appointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(
                x => x.Id == request.AppointmentId && !x.IsDeleted,
                include: q => q.Include(a => a.Business)
            );

        if (appointment == null)
        {
            return ApiResponse<CancelAppointmentByBusinessCommandResponse>.NotFoundResult("Randevu bulunamadı.");
        }

        if (appointment.Business?.AppUserId != _currentUserService.UserId.Value)
        {
            return ApiResponse<CancelAppointmentByBusinessCommandResponse>.ForbiddenResult("Bu randevuyu iptal etme yetkiniz yok.");
        }

        if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Confirmed)
        {
            return ApiResponse<CancelAppointmentByBusinessCommandResponse>.FailResult("Bu randevu iptal edilemez.");
        }

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.BusinessNotes = request.BusinessNotes;

        await _unitOfWork.GetWriteRepository<Appointment>().UpdateAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CancelAppointmentByBusinessCommandResponse>.SuccessResult(
            new CancelAppointmentByBusinessCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu başarıyla iptal edildi.");
    }
}
