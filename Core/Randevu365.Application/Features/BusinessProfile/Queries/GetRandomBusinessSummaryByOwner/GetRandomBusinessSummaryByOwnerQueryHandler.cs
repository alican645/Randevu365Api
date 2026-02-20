using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetRandomBusinessSummaryByOwner;

public class GetRandomBusinessSummaryByOwnerQueryHandler : IRequestHandler<GetRandomBusinessSummaryByOwnerQueryRequest, ApiResponse<GetRandomBusinessSummaryByOwnerQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetRandomBusinessSummaryByOwnerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetRandomBusinessSummaryByOwnerQueryResponse>> Handle(GetRandomBusinessSummaryByOwnerQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            return ApiResponse<GetRandomBusinessSummaryByOwnerQueryResponse>.UnauthorizedResult();
        }

        var businesses = await _unitOfWork.GetReadRepository<Business>()
            .GetAllAsync(
                predicate: x => x.AppUserId == currentUserId.Value && !x.IsDeleted,
                include: q => q
                    .Include(b => b.BusinessRatings)
                    .Include(b => b.BusinessComments)
                    .Include(b => b.BusinessLogo)
                    .Include(b => b.BusinessPhotos)
                    .Include(b => b.Appointments)
            );

        if (businesses == null || !businesses.Any())
        {
            return ApiResponse<GetRandomBusinessSummaryByOwnerQueryResponse>.NotFoundResult();
        }

        var randomIndex = new Random().Next(businesses.Count);
        var business = businesses[randomIndex];

        var response = new GetRandomBusinessSummaryByOwnerQueryResponse
        {
            BusinessId = business.Id,
            BusinessName = business.BusinessName,
            AverageRating = business.BusinessRatings?.Any() == true
                ? (decimal)business.BusinessRatings.Average(r => r.Rating)
                : 0,
            CommentCount = business.BusinessComments?.Count(c => !c.IsDeleted) ?? 0,
            LogoUrl = business.BusinessLogo?.LogoUrl,
            FirstPhotoPath = business.BusinessPhotos?
                .Where(p => p.IsActive)
                .OrderBy(p => p.Id)
                .FirstOrDefault()?.PhotoPath,
            AppointmentCount = business.Appointments?.Count(a => !a.IsDeleted) ?? 0
        };

        return ApiResponse<GetRandomBusinessSummaryByOwnerQueryResponse>.SuccessResult(response);
    }
}
