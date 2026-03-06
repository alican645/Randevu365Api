using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.RevertConfirmedAppointment;

public class RevertConfirmedAppointmentCommandHandler : IRequestHandler<RevertConfirmedAppointmentCommandRequest, ApiResponse<RevertConfirmedAppointmentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RevertConfirmedAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<RevertConfirmedAppointmentCommandResponse>> Handle(RevertConfirmedAppointmentCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<RevertConfirmedAppointmentCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var appointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(
                x => x.Id == request.AppointmentId && !x.IsDeleted,
                include: q => q.Include(a => a.Business)
            );

        if (appointment == null)
        {
            return ApiResponse<RevertConfirmedAppointmentCommandResponse>.NotFoundResult("Randevu bulunamadı.");
        }

        if (appointment.Business?.AppUserId != _currentUserService.UserId.Value)
        {
            return ApiResponse<RevertConfirmedAppointmentCommandResponse>.ForbiddenResult("Bu randevuyu geri alma yetkiniz yok.");
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return ApiResponse<RevertConfirmedAppointmentCommandResponse>.FailResult("Yalnızca onaylanmış randevular geri alınabilir.");
        }
        
        appointment.Status = AppointmentStatus.Rejected;

        await _unitOfWork.GetWriteRepository<Appointment>().UpdateAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<RevertConfirmedAppointmentCommandResponse>.SuccessResult(
            new RevertConfirmedAppointmentCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu onayı başarıyla geri alındı.");
    }
}
