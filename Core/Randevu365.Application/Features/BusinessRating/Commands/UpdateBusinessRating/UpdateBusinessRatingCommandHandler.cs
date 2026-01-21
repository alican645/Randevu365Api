using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.BusinessRating.Commands.UpdateBusinessRating;

public class UpdateBusinessRatingCommandHandler : IRequestHandler<UpdateBusinessRatingCommandRequest, ApiResponse<UpdateBusinessRatingCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBusinessRatingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<UpdateBusinessRatingCommandResponse>> Handle(UpdateBusinessRatingCommandRequest request, CancellationToken cancellationToken)
    {
   
        var validator = new UpdateBusinessRatingCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<UpdateBusinessRatingCommandResponse>.FailResult(errors);
        }

        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return ApiResponse<UpdateBusinessRatingCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var rating = await _unitOfWork.GetReadRepository<Domain.Entities.BusinessRating>().GetAsync(x => x.Id == request.RatingId);

        if (rating == null)
        {
            return ApiResponse<UpdateBusinessRatingCommandResponse>.NotFoundResult("Puan bulunamadı.");
        }

        // Sadece kendi puanını güncelleyebilir
        if (rating.AppUserId != userId.Value)
        {
            return ApiResponse<UpdateBusinessRatingCommandResponse>.ForbiddenResult("Bu puanı güncelleme yetkiniz yok.");
        }

        rating.Rating = request.Rating;

        await _unitOfWork.GetWriteRepository<Domain.Entities.BusinessRating>().UpdateAsync(rating);
        await _unitOfWork.SaveAsync();

        var response = new UpdateBusinessRatingCommandResponse
        {
            Id = rating.Id,
            BusinessId = rating.BusinessId,
            AppUserId = rating.AppUserId,
            Rating = rating.Rating
        };

        return ApiResponse<UpdateBusinessRatingCommandResponse>.SuccessResult(response, "Puan başarıyla güncellendi.");
    }
}
