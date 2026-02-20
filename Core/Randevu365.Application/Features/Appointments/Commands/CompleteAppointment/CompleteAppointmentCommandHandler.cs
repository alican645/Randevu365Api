using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommandRequest, ApiResponse<CompleteAppointmentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CompleteAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CompleteAppointmentCommandResponse>> Handle(CompleteAppointmentCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<CompleteAppointmentCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var appointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(
                x => x.Id == request.AppointmentId && !x.IsDeleted,
                include: q => q.Include(a => a.Business)
            );

        if (appointment == null)
        {
            return ApiResponse<CompleteAppointmentCommandResponse>.NotFoundResult("Randevu bulunamadı.");
        }

        if (appointment.Business?.AppUserId != _currentUserService.UserId.Value)
        {
            return ApiResponse<CompleteAppointmentCommandResponse>.ForbiddenResult("Bu randevuyu tamamlama yetkiniz yok.");
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return ApiResponse<CompleteAppointmentCommandResponse>.FailResult("Yalnızca onaylanmış randevular tamamlanabilir.");
        }

        appointment.Status = AppointmentStatus.Completed;

        await _unitOfWork.GetWriteRepository<Appointment>().UpdateAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CompleteAppointmentCommandResponse>.SuccessResult(
            new CompleteAppointmentCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu başarıyla tamamlandı.");
    }
}
