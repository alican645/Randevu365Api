using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Favorites.Queries.GetMyFavorites;

public class GetMyFavoritesQueryHandler : IRequestHandler<GetMyFavoritesQueryRequest, ApiResponse<List<GetMyFavoritesQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyFavoritesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<List<GetMyFavoritesQueryResponse>>> Handle(GetMyFavoritesQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<List<GetMyFavoritesQueryResponse>>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var favorites = await _unitOfWork.GetReadRepository<Favorite>()
            .GetAllAsync(
                predicate: f => f.AppUserId == _currentUserService.UserId.Value && !f.IsDeleted,
                include: q => q.Include(f => f.Business!),
                orderBy: q => q.OrderByDescending(f => f.CreatedAt));

        var response = favorites.Select(f => new GetMyFavoritesQueryResponse
        {
            FavoriteId = f.Id,
            BusinessId = f.BusinessId,
            BusinessName = f.Business?.BusinessName ?? string.Empty,
            BusinessCity = f.Business?.BusinessCity ?? string.Empty,
            BusinessCategory = f.Business?.BusinessCategory?.ToString(),
            AddedAt = f.CreatedAt
        }).ToList();

        return ApiResponse<List<GetMyFavoritesQueryResponse>>.SuccessResult(response);
    }
}
