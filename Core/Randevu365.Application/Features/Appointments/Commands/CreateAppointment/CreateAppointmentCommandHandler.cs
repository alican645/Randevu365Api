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

        var dayOfWeek = request.AppointmentDate.DayOfWeek;
        var dayNameEn = dayOfWeek.ToString();
        var dayNameTr = dayOfWeek switch
        {
            DayOfWeek.Monday => "Pazartesi",
            DayOfWeek.Tuesday => "Salı",
            DayOfWeek.Wednesday => "Çarşamba",
            DayOfWeek.Thursday => "Perşembe",
            DayOfWeek.Friday => "Cuma",
            DayOfWeek.Saturday => "Cumartesi",
            DayOfWeek.Sunday => "Pazar",
            _ => dayNameEn
        };

        var businessHour = await _unitOfWork.GetReadRepository<BusinessHour>()
            .GetAsync(x => x.BusinessId == request.BusinessId
                          && (x.Day == dayNameEn || x.Day == dayNameTr)
                          && !x.IsDeleted);

        if (businessHour == null)
        {
            return ApiResponse<CreateAppointmentCommandResponse>.FailResult(
                "İşletme seçilen günde hizmet vermemektedir.");
        }

        var openTime = TimeOnly.Parse(businessHour.OpenTime);
        var closeTime = TimeOnly.Parse(businessHour.CloseTime);

        if (request.RequestedStartTime < openTime || request.RequestedEndTime > closeTime)
        {
            return ApiResponse<CreateAppointmentCommandResponse>.FailResult(
                $"Randevu saatleri işletmenin çalışma saatleri ({businessHour.OpenTime} - {businessHour.CloseTime}) dışındadır.");
        }

        var userId = _currentUserService.UserId.Value;

        var userDuplicateAppointment = await _unitOfWork.GetReadRepository<Appointment>()
            .GetAsync(x => x.AppUserId == userId
                          && x.BusinessId == request.BusinessId
                          && x.AppointmentDate == request.AppointmentDate
                          && x.RequestedStartTime < request.RequestedEndTime
                          && x.RequestedEndTime > request.RequestedStartTime
                          && (x.Status == AppointmentStatus.Pending || x.Status == AppointmentStatus.Confirmed)
                          && !x.IsDeleted);

        if (userDuplicateAppointment != null)
        {
            return ApiResponse<CreateAppointmentCommandResponse>.ConflictResult(
                "Bu işletmede seçilen zaman diliminde zaten bir randevunuz bulunmaktadır.");
        }

        var overlappingCount = await _unitOfWork.GetReadRepository<Appointment>().CountAsync(
            x => x.BusinessServiceId == request.BusinessServiceId
                 && x.AppointmentDate == request.AppointmentDate
                 && x.RequestedStartTime < request.RequestedEndTime
                 && x.RequestedEndTime > request.RequestedStartTime
                 && (x.Status == AppointmentStatus.Pending || x.Status == AppointmentStatus.Confirmed)
                 && !x.IsDeleted
        );

        if (overlappingCount >= businessService.MaxConcurrentCustomers)
        {
            return ApiResponse<CreateAppointmentCommandResponse>.ConflictResult("Seçilen zaman diliminde kapasite dolmuştur.");
        }

        var appointment = new Appointment
        {
            AppUserId = userId,
            BusinessId = request.BusinessId,
            BusinessServiceId = request.BusinessServiceId,
            AppointmentDate = request.AppointmentDate,
            RequestedStartTime = request.RequestedStartTime,
            RequestedEndTime = request.RequestedEndTime,
            CustomerNotes = request.CustomerNotes,
            Status = AppointmentStatus.Pending
        };

        await _unitOfWork.GetWriteRepository<Appointment>().AddAsync(appointment);
        await _unitOfWork.SaveAsync();

        // Randevu için otomatik conversation oluştur
        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(b => b.Id == request.BusinessId && !b.IsDeleted);

        if (business != null)
        {
            var conversation = new Conversation
            {
                UserId = userId,
                OtherUserId = business.AppUserId,
                AppointmentId = appointment.Id,
                ConversationId = $"apt_{appointment.Id}",
                IsClosed = false
            };
            await _unitOfWork.GetWriteRepository<Conversation>().AddAsync(conversation);
            await _unitOfWork.SaveAsync();
        }

        return ApiResponse<CreateAppointmentCommandResponse>.CreatedResult(
            new CreateAppointmentCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu başarıyla oluşturuldu.");
    }
}
