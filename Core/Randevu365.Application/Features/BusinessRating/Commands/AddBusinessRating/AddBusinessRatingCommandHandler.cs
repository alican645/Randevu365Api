using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessRating.Commands.AddBusinessRating;

public class AddBusinessRatingCommandHandler : IRequestHandler<AddBusinessRatingCommandRequest, ApiResponse<AddBusinessRatingCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddBusinessRatingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<AddBusinessRatingCommandResponse>> Handle(AddBusinessRatingCommandRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return ApiResponse<AddBusinessRatingCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        if (request.Rating < 1 || request.Rating > 5)
        {
            return ApiResponse<AddBusinessRatingCommandResponse>.FailResult("Puan 1 ile 5 arasında olmalıdır.");
        }

        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(x => x.Id == request.BusinessId);
        if (business == null)
        {
            return ApiResponse<AddBusinessRatingCommandResponse>.NotFoundResult("İşletme bulunamadı.");
        }

        // Kullanıcının daha önce puan verip vermediğini kontrol et
        var existingRating = await _unitOfWork.GetReadRepository<Domain.Entities.BusinessRating>().GetAsync(
            x => x.BusinessId == request.BusinessId && x.AppUserId == userId.Value);

        if (existingRating != null)
        {
            return ApiResponse<AddBusinessRatingCommandResponse>.FailResult("Bu işletmeye zaten puan verdiniz. Puanınızı güncelleyebilirsiniz.");
        }

        var rating = new Domain.Entities.BusinessRating
        {
            BusinessId = request.BusinessId,
            AppUserId = userId.Value,
            Rating = request.Rating
        };

        await _unitOfWork.GetWriteRepository<Domain.Entities.BusinessRating>().AddAsync(rating);
        await _unitOfWork.SaveAsync();

        var response = new AddBusinessRatingCommandResponse
        {
            Id = rating.Id,
            BusinessId = rating.BusinessId,
            AppUserId = rating.AppUserId,
            Rating = rating.Rating,
            CreatedAt = rating.CreatedAt
        };

        return ApiResponse<AddBusinessRatingCommandResponse>.CreatedResult(response, "Puan başarıyla eklendi.");
    }
}
