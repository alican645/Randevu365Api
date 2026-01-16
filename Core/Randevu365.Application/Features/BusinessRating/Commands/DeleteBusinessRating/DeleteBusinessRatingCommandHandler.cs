using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.BusinessRating.Commands.DeleteBusinessRating;

public class DeleteBusinessRatingCommandHandler : IRequestHandler<DeleteBusinessRatingCommandRequest, ApiResponse<DeleteBusinessRatingCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteBusinessRatingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<DeleteBusinessRatingCommandResponse>> Handle(DeleteBusinessRatingCommandRequest request, CancellationToken cancellationToken)
    {

        var validator = new DeleteBusinessRatingCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<DeleteBusinessRatingCommandResponse>.FailResult(errors);
        }

        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return ApiResponse<DeleteBusinessRatingCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var rating = await _unitOfWork.GetReadRepository<Domain.Entities.BusinessRating>().GetAsync(x => x.Id == request.RatingId);

        if (rating == null)
        {
            return ApiResponse<DeleteBusinessRatingCommandResponse>.NotFoundResult("Puan bulunamadı.");
        }

        if (rating.AppUserId != userId.Value)
        {
            return ApiResponse<DeleteBusinessRatingCommandResponse>.ForbiddenResult("Bu puanı silme yetkiniz yok.");
        }

        await _unitOfWork.GetWriteRepository<Domain.Entities.BusinessRating>().HardDeleteAsync(rating);
        await _unitOfWork.SaveAsync();

        var response = new DeleteBusinessRatingCommandResponse
        {
            Id = request.RatingId
        };

        return ApiResponse<DeleteBusinessRatingCommandResponse>.SuccessResult(response, "Puan başarıyla silindi.");
    }
}
