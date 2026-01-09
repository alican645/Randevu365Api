using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.BusinessRating.Queries.GetMyRatings;

public class GetMyRatingsQueryHandler : IRequestHandler<GetMyRatingsQueryRequest, ApiResponse<IList<GetMyRatingsQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyRatingsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<IList<GetMyRatingsQueryResponse>>> Handle(GetMyRatingsQueryRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return ApiResponse<IList<GetMyRatingsQueryResponse>>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var ratings = await _unitOfWork.GetReadRepository<Domain.Entities.BusinessRating>().GetAllAsync(
            predicate: x => x.AppUserId == userId,
            include: q => q.Include(r => r.Business)
        );

        var response = ratings.Select(r => new GetMyRatingsQueryResponse
        {
            Id = r.Id,
            BusinessId = r.BusinessId,
            BusinessName = r.Business?.BusinessName,
            Rating = r.Rating,
            CreatedAt = r.CreatedAt
        }).ToList();

        return ApiResponse<IList<GetMyRatingsQueryResponse>>.SuccessResult(response);
    }
}
