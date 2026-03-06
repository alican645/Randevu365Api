using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CreateAppointmentByOwner;

public class CreateAppointmentByOwnerCommandHandler : IRequestHandler<CreateAppointmentByOwnerCommandRequest, ApiResponse<CreateAppointmentByOwnerCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateAppointmentByOwnerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CreateAppointmentByOwnerCommandResponse>> Handle(CreateAppointmentByOwnerCommandRequest request, CancellationToken cancellationToken)
    {
        // 1. Validasyon
        var validator = new CreateAppointmentByOwnerCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<CreateAppointmentByOwnerCommandResponse>.FailResult(errors);
        }

        // 2. Yetki Kontrolü (İşletme sahibi mi?)
        if (_currentUserService.UserId is null)
        {
            return ApiResponse<CreateAppointmentByOwnerCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(x => x.Id == request.BusinessId && !x.IsDeleted);

        if (business == null)
        {
            return ApiResponse<CreateAppointmentByOwnerCommandResponse>.NotFoundResult("İşletme bulunamadı.");
        }

        if (business.AppUserId != _currentUserService.UserId.Value)
        {
            return ApiResponse<CreateAppointmentByOwnerCommandResponse>.ForbiddenResult("Bu işletme için randevu oluşturma yetkiniz yok.");
        }

        // 3. Müşteri Kontrolü
        var customerExists = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>()
            .GetAsync(x => x.Id == request.AppUserId && !x.IsDeleted);

        if (customerExists == null)
        {
            return ApiResponse<CreateAppointmentByOwnerCommandResponse>.NotFoundResult("Belirtilen müşteri bulunamadı.");
        }

        // 4. Hizmet Kontrolü
        var businessService = await _unitOfWork.GetReadRepository<BusinessService>()
            .GetAsync(x => x.Id == request.BusinessServiceId && x.BusinessId == request.BusinessId && !x.IsDeleted);

        if (businessService == null)
        {
            return ApiResponse<CreateAppointmentByOwnerCommandResponse>.NotFoundResult("Hizmet bu işletmeye ait değil veya bulunamadı.");
        }

        // 5. Kapasite/Overlap Kontrolü
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
            return ApiResponse<CreateAppointmentByOwnerCommandResponse>.ConflictResult("Seçilen zaman diliminde kapasite dolmuştur.");
        }

        // 6. Randevu Oluşturma (Otomatik Onaylı)
        var appointment = new Appointment
        {
            AppUserId = request.AppUserId, // Seçilen müşteri ID'si
            BusinessId = request.BusinessId,
            BusinessServiceId = request.BusinessServiceId,
            AppointmentDate = request.AppointmentDate,
            RequestedStartTime = request.RequestedStartTime,
            RequestedEndTime = request.RequestedEndTime,
            ApproveStartTime = request.RequestedStartTime, // Owner oluşturduğu için direkt approve saatleri olarak set edilebilir
            ApproveEndTime = request.RequestedEndTime,
            CustomerNotes = request.CustomerNotes,
            BusinessNotes = request.BusinessNotes,
            Status = AppointmentStatus.Confirmed // Otomatik onaylı
        };

        await _unitOfWork.GetWriteRepository<Appointment>().AddAsync(appointment);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateAppointmentByOwnerCommandResponse>.CreatedResult(
            new CreateAppointmentByOwnerCommandResponse { Id = appointment.Id, Status = appointment.Status },
            "Randevu işletme sahibi tarafından başarıyla oluşturuldu ve onaylandı.");
    }
}
