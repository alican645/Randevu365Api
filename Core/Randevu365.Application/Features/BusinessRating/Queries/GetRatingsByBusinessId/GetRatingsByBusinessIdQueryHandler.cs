using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.BusinessRating.Queries.GetRatingsByBusinessId;

public class GetRatingsByBusinessIdQueryHandler : IRequestHandler<GetRatingsByBusinessIdQueryRequest, ApiResponse<IList<GetRatingsByBusinessIdQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRatingsByBusinessIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IList<GetRatingsByBusinessIdQueryResponse>>> Handle(GetRatingsByBusinessIdQueryRequest request, CancellationToken cancellationToken)
    {
        
        var validator = new GetRatingsByBusinessIdQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<IList<GetRatingsByBusinessIdQueryResponse>>.FailResult(errors);
        }

        var ratings = await _unitOfWork.GetReadRepository<Domain.Entities.BusinessRating>().GetAllAsync(
            predicate: x => x.BusinessId == request.BusinessId,
            include: q => q.Include(r => r.AppUser).ThenInclude(u => u!.AppUserInformation)
        );

        var response = ratings.Select(r => new GetRatingsByBusinessIdQueryResponse
        {
            Id = r.Id,
            BusinessId = r.BusinessId,
            AppUserId = r.AppUserId,
            Rating = r.Rating,
            UserName = r.AppUser?.AppUserInformation?.Name,
            CreatedAt = r.CreatedAt
        }).ToList();

        return ApiResponse<IList<GetRatingsByBusinessIdQueryResponse>>.SuccessResult(response);
    }
}
