using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

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
        var business = new Business
        {
            BusinessName = request.BusinessName,
            BusinessAddress = request.BusinessAddress,
            BusinessCity = request.BusinessCity,
            BusinessPhone = request.BusinessPhone,
            BusinessEmail = request.BusinessEmail,
            BusinessCountry = request.BusinessCountry,
            AppUserId = _currentUserService.UserId.Value
        };

        await _unitOfWork.GetWriteRepository<Business>().AddAsync(business);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateBusinessCommandResponse>.SuccessResult(
            new CreateBusinessCommandResponse { Id = business.Id },
            "İşletme başarıyla oluşturuldu.");
    }
}
