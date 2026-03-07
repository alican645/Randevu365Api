using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Favorites.Commands.AddFavorite;

public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommandRequest, ApiResponse<AddFavoriteCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddFavoriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<AddFavoriteCommandResponse>> Handle(AddFavoriteCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<AddFavoriteCommandResponse>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var userId = _currentUserService.UserId.Value;

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(b => b.Id == request.BusinessId && !b.IsDeleted);
        if (business == null)
            return ApiResponse<AddFavoriteCommandResponse>.NotFoundResult("Isletme bulunamadi.");

        var existingFavorite = await _unitOfWork.GetReadRepository<Favorite>()
            .GetAsync(f => f.AppUserId == userId && f.BusinessId == request.BusinessId && !f.IsDeleted);
        if (existingFavorite != null)
            return ApiResponse<AddFavoriteCommandResponse>.ConflictResult("Bu isletme zaten favorilerinizde.");

        var favorite = new Favorite
        {
            AppUserId = userId,
            BusinessId = request.BusinessId
        };

        await _unitOfWork.GetWriteRepository<Favorite>().AddAsync(favorite);
        await _unitOfWork.SaveAsync();

        return ApiResponse<AddFavoriteCommandResponse>.CreatedResult(
            new AddFavoriteCommandResponse { Id = favorite.Id },
            "Isletme favorilere eklendi.");
    }
}
