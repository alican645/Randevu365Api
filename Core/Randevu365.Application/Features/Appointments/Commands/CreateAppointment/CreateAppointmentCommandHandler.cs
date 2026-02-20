using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommandRequest, ApiResponse<CreateAppointmentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateAppointmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CreateAppointmentCommandResponse>> Handle(CreateAppointmentCommandRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateAppointmentCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<CreateAppointmentCommandResponse>.FailResult(errors);
        }

        if (_currentUserService.UserId is null)
        {
            return ApiResponse<CreateAppointmentCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var businessService = await _unitOfWork.GetReadRepository<BusinessService>()
            .GetAsync(x => x.Id == request.BusinessServiceId && x.BusinessId == request.BusinessId && !x.IsDeleted);

        if (businessService == null)
        {
            return ApiResponse<CreateAppointmentCommandResponse>.NotFoundResult("Hizmet bulunamadı.");
        }

        var overlappingCount = await _unitOfWork.GetReadRepository<Appointment>().CountAsync(
            x => x.BusinessServiceId == request.BusinessServiceId
                 && x.AppointmentDate == request.AppointmentDate
                 && x.StartTime < request.EndTime
                 && x.EndTime > request.StartTime
                 && (x.Status == AppointmentStatus.Pending || x.Status == AppointmentStatus.Confirmed)
                 && !x.IsDeleted
        );

        if (overlappingCount >= businessService.MaxConcurrentCustomers)
        {
            return ApiResponse<CreateAppointmentCommandResponse>.ConflictResult("Seçilen zaman diliminde kapasite dolmuştur.");
        }

        var appointment = new Appointment
        {
            AppUserId = _currentUserService.UserId.Value,
            BusinessId = request.BusinessId,
            BusinessServiceId = request.BusinessServiceId,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            CustomerNotes = request.CustomerNotes,
            Status = AppointmentStatus.Pending
        };

        await _unitOfWork.GetWriteRepository<Appointment>().AddAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateAppointmentCommandResponse>.CreatedResult(
            new CreateAppointmentCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu başarıyla oluşturuldu.");
    }
}
