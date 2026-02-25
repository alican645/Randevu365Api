using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;
using BusinessLogoEntity = Randevu365.Domain.Entities.BusinessLogo;

namespace Randevu365.Application.Features.Businesses.Commands.CreateBusinessDetail;

public class CreateBusinessDetailCommandHandler : IRequestHandler<CreateBusinessDetailCommandRequest, ApiResponse<CreateBusinessDetailCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;

    public CreateBusinessDetailCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileService = fileService;
    }

    public async Task<ApiResponse<CreateBusinessDetailCommandResponse>> Handle(CreateBusinessDetailCommandRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateBusinessDetailCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<CreateBusinessDetailCommandResponse>.FailResult(errors);
        }

        if (_currentUserService.UserId is null)
        {
            return ApiResponse<CreateBusinessDetailCommandResponse>.UnauthorizedResult("Kullanıcı oturumu bulunamadı.");
        }

        var currentUserId = _currentUserService.UserId.Value;

        var existingBusinesses = await _unitOfWork.GetReadRepository<Business>()
            .GetAllAsync(predicate: x => x.AppUserId == currentUserId && !x.IsDeleted);

        BusinessSlot? availableSlot = null;

        if (existingBusinesses.Count >= 1)
        {
            availableSlot = await _unitOfWork.GetReadRepository<BusinessSlot>()
                .GetAsync(predicate: x => x.AppUserId == currentUserId &&
                                          !x.IsUsed &&
                                          x.PaymentStatus == SlotPaymentStatus.Completed &&
                                          !x.IsDeleted);

            if (availableSlot == null)
                return ApiResponse<CreateBusinessDetailCommandResponse>.PaymentRequiredResult(
                    "Yeni bir işyeri eklemek için önce işyeri slotu satın almanız gerekiyor.",
                    new[] { "Yeni bir işyeri eklemek için önce işyeri slotu satın almanız gerekiyor." });
        }

        BusinessCategory? category = null;
        if (BusinessCategoryExtensions.TryFromJson(request.BusinessCategory, out var cat))
            category = cat;

        var business = new Business
        {
            BusinessName = request.BusinessName!,
            BusinessAddress = request.BusinessAddress!,
            BusinessCity = request.BusinessCity!,
            BusinessPhone = request.BusinessPhone!,
            BusinessEmail = request.BusinessEmail!,
            BusinessCountry = request.BusinessCountry!,
            BusinessCategory = category,
            AppUserId = currentUserId
        };

        await _unitOfWork.GetWriteRepository<Business>().AddAsync(business);
        await _unitOfWork.SaveAsync();

        // Business Logo
        if (request.BusinessLogo != null)
        {
            var logoUrl = await _fileService.UploadFileAsync(request.BusinessLogo, $"business/{business.Id}/logo");
            var logo = new BusinessLogoEntity
            {
                LogoUrl = logoUrl,
                BusinessId = business.Id
            };
            await _unitOfWork.GetWriteRepository<BusinessLogoEntity>().AddAsync(logo);
        }

        // Business Hours
        if (request.BusinessHours is { Count: > 0 })
        {
            var hours = request.BusinessHours.Select(h => new BusinessHour
            {
                Day = h.Day!,
                OpenTime = h.OpenTime!,
                CloseTime = h.CloseTime!,
                BusinessId = business.Id
            }).ToList();

            await _unitOfWork.GetWriteRepository<BusinessHour>().AddRangeAsync(hours);
        }

        // Business Services
        if (request.BusinessServices is { Count: > 0 })
        {
            var services = request.BusinessServices.Select(s => new BusinessService
            {
                ServiceTitle = s.ServiceTitle!,
                ServiceContent = s.ServiceContent!,
                MaxConcurrentCustomers = s.MaxConcurrentCustomers,
                ServicePrice =  s.ServicePrice,
                BusinessId = business.Id
            }).ToList();

            await _unitOfWork.GetWriteRepository<BusinessService>().AddRangeAsync(services);
        }

        // Business Location
        if (request.Location != null)
        {
            var location = new BusinessLocation(business.Id, request.Location.Latitude, request.Location.Longitude);
            await _unitOfWork.GetWriteRepository<BusinessLocation>().AddAsync(location);
        }

        // Business Photos
        if (request.BusinessPhotos is { Count: > 0 })
        {
            var photos = new List<BusinessPhoto>();
            foreach (var file in request.BusinessPhotos)
            {
                var photoPath = await _fileService.UploadFileAsync(file, $"business/{business.Id}/photos");
                photos.Add(new BusinessPhoto
                {
                    PhotoPath = photoPath,
                    IsActive = true,
                    BusinessId = business.Id
                });
            }

            await _unitOfWork.GetWriteRepository<BusinessPhoto>().AddRangeAsync(photos);
        }

        await _unitOfWork.SaveAsync();

        if (availableSlot != null)
        {
            availableSlot.IsUsed = true;
            availableSlot.UsedForBusinessId = business.Id;
            availableSlot.UsedAt = DateTime.UtcNow;
            await _unitOfWork.GetWriteRepository<BusinessSlot>().UpdateAsync(availableSlot);
            await _unitOfWork.SaveAsync();
        }

        return ApiResponse<CreateBusinessDetailCommandResponse>.CreatedResult(
            new CreateBusinessDetailCommandResponse { Id = business.Id },
            "İşletme detayları başarıyla oluşturuldu.");
    }
}
