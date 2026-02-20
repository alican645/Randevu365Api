using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointmentByCustomer;

public class CancelAppointmentByCustomerCommandHandler : IRequestHandler<CancelAppointmentByCustomerCommandRequest, ApiResponse<CancelAppointmentByCustomerCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CancelAppointmentByCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CancelAppointmentByCustomerCommandResponse>> Handle(CancelAppointmentByCustomerCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<CancelAppointmentByCustomerCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var appointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(x => x.Id == request.AppointmentId && !x.IsDeleted);

        if (appointment == null)
        {
            return ApiResponse<CancelAppointmentByCustomerCommandResponse>.NotFoundResult("Randevu bulunamadı.");
        }

        if (appointment.AppUserId != _currentUserService.UserId.Value)
        {
            return ApiResponse<CancelAppointmentByCustomerCommandResponse>.ForbiddenResult("Bu randevuyu iptal etme yetkiniz yok.");
        }

        if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Confirmed)
        {
            return ApiResponse<CancelAppointmentByCustomerCommandResponse>.FailResult("Bu randevu iptal edilemez.");
        }

        appointment.Status = AppointmentStatus.Cancelled;

        await _unitOfWork.GetWriteRepository<Appointment>().UpdateAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CancelAppointmentByCustomerCommandResponse>.SuccessResult(
            new CancelAppointmentByCustomerCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu başarıyla iptal edildi.");
    }
}
