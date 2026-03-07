using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Favorites.Commands.RemoveFavorite;

public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommandRequest, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RemoveFavoriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse> Handle(RemoveFavoriteCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return new ApiResponse { Success = false, StatusCode = 401, Message = "Kullanici kimliği bulunamadi." };

        var favorite = await _unitOfWork.GetReadRepository<Favorite>()
            .GetAsync(f => f.AppUserId == _currentUserService.UserId.Value && f.BusinessId == request.BusinessId && !f.IsDeleted, enableTracking: true);

        if (favorite == null)
            return new ApiResponse { Success = false, StatusCode = 404, Message = "Favori bulunamadi." };

        await _unitOfWork.GetWriteRepository<Favorite>().HardDeleteAsync(favorite);
        await _unitOfWork.SaveAsync();

        return new ApiResponse { Success = true, StatusCode = 200, Message = "Isletme favorilerden kaldirildi." };
    }
}
