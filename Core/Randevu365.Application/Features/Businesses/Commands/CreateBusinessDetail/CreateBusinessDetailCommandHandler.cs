using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using BusinessLogoEntity = Randevu365.Domain.Entities.BusinessLogo;

namespace Randevu365.Application.Features.Businesses.Commands.CreateBusinessDetail;

public class CreateBusinessDetailCommandHandler : IRequestHandler<CreateBusinessDetailCommandRequest, ApiResponse<CreateBusinessDetailCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateBusinessDetailCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
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

        var existingBusiness = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(x => x.AppUserId == _currentUserService.UserId.Value);

        if (existingBusiness != null)
        {
            return ApiResponse<CreateBusinessDetailCommandResponse>.ConflictResult("Bu kullanıcıya ait bir işletme zaten mevcut.");
        }

        var business = new Business
        {
            BusinessName = request.BusinessName!,
            BusinessAddress = request.BusinessAddress!,
            BusinessCity = request.BusinessCity!,
            BusinessPhone = request.BusinessPhone!,
            BusinessEmail = request.BusinessEmail!,
            BusinessCountry = request.BusinessCountry!,
            AppUserId = _currentUserService.UserId.Value
        };

        await _unitOfWork.GetWriteRepository<Business>().AddAsync(business);
        await _unitOfWork.SaveAsync();

        // Business Logo
        if (!string.IsNullOrWhiteSpace(request.BusinessLogo))
        {
            var logo = new BusinessLogoEntity
            {
                LogoUrl = request.BusinessLogo,
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

        // Business Photos
        if (request.BusinessPhotos is { Count: > 0 })
        {
            var photos = request.BusinessPhotos.Select(p => new BusinessPhoto
            {
                PhotoPath = p,
                IsActive = true,
                BusinessId = business.Id
            }).ToList();

            await _unitOfWork.GetWriteRepository<BusinessPhoto>().AddRangeAsync(photos);
        }

        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateBusinessDetailCommandResponse>.CreatedResult(
            new CreateBusinessDetailCommandResponse { Id = business.Id },
            "İşletme detayları başarıyla oluşturuldu.");
    }
}
