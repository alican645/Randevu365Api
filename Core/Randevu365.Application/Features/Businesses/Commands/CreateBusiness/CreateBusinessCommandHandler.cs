using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Commands.CreateBusiness;

public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommandRequest, ApiResponse<CreateBusinessCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateBusinessCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CreateBusinessCommandResponse>> Handle(CreateBusinessCommandRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateBusinessCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<CreateBusinessCommandResponse>.FailResult(errors);
        }

        if (_currentUserService.UserId is null)
        {
            return ApiResponse<CreateBusinessCommandResponse>.UnauthorizedResult("Kullanıcı oturumu bulunamadı.");
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
                return ApiResponse<CreateBusinessCommandResponse>.PaymentRequiredResult(
                    "Yeni bir işyeri eklemek için önce işyeri slotu satın almanız gerekiyor.");
        }

        BusinessCategory? category = null;
        if (BusinessCategoryExtensions.TryFromJson(request.BusinessCategory, out var cat))
            category = cat;

        var business = new Business
        {
            BusinessName = request.BusinessName,
            BusinessAddress = request.BusinessAddress,
            BusinessCity = request.BusinessCity,
            BusinessPhone = request.BusinessPhone,
            BusinessEmail = request.BusinessEmail,
            BusinessCountry = request.BusinessCountry,
            BusinessCategory = category,
            AppUserId = currentUserId
        };

        await _unitOfWork.GetWriteRepository<Business>().AddAsync(business);
        await _unitOfWork.SaveAsync();

        if (availableSlot != null)
        {
            availableSlot.IsUsed = true;
            availableSlot.UsedForBusinessId = business.Id;
            availableSlot.UsedAt = DateTime.UtcNow;
            await _unitOfWork.GetWriteRepository<BusinessSlot>().UpdateAsync(availableSlot);
            await _unitOfWork.SaveAsync();
        }

        return ApiResponse<CreateBusinessCommandResponse>.CreatedResult(
            new CreateBusinessCommandResponse { Id = business.Id },
            "İşletme başarıyla oluşturuldu.");
    }
}
