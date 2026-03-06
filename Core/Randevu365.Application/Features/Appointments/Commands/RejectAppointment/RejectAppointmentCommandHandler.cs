using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.RejectAppointment;

public class RejectAppointmentCommandHandler : IRequestHandler<RejectAppointmentCommandRequest, ApiResponse<RejectAppointmentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RejectAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<RejectAppointmentCommandResponse>> Handle(RejectAppointmentCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<RejectAppointmentCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var appointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(
                x => x.Id == request.AppointmentId && !x.IsDeleted,
                include: q => q.Include(a => a.Business)
            );

        if (appointment == null)
        {
            return ApiResponse<RejectAppointmentCommandResponse>.NotFoundResult("Randevu bulunamadı.");
        }

        if (appointment.Business?.AppUserId != _currentUserService.UserId.Value)
        {
            return ApiResponse<RejectAppointmentCommandResponse>.ForbiddenResult("Bu randevuyu reddetme yetkiniz yok.");
        }

        if (appointment.Status != AppointmentStatus.Pending)
        {
            return ApiResponse<RejectAppointmentCommandResponse>.FailResult("Yalnızca beklemedeki randevular reddedilebilir.");
        }

        appointment.Status = AppointmentStatus.Rejected;

        if (!string.IsNullOrWhiteSpace(request.BusinessNotes))
        {
            appointment.BusinessNotes = request.BusinessNotes;
        }

        await _unitOfWork.GetWriteRepository<Appointment>().UpdateAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<RejectAppointmentCommandResponse>.SuccessResult(
            new RejectAppointmentCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu başarıyla reddedildi.");
    }
}
