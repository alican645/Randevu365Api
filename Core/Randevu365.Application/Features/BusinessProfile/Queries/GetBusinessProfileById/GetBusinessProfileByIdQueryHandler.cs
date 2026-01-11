using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileById;

public class GetBusinessProfileByIdQueryHandler : IRequestHandler<GetBusinessProfileByIdQueryRequest, ApiResponse<GetBusinessProfileByIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessProfileByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetBusinessProfileByIdQueryResponse>> Handle(GetBusinessProfileByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(
            predicate: x => x.Id == request.Id,
            include: q => q.Include(b => b.BusinessLocations).Include(b => b.BusinessPhotos).Include(b => b.BusinessRatings)
        );

        if (business == null)
        {
            return ApiResponse<GetBusinessProfileByIdQueryResponse>.NotFoundResult("Business profile not found.");
        }

        return ApiResponse<GetBusinessProfileByIdQueryResponse>.SuccessResult(new GetBusinessProfileByIdQueryResponse { Business = business });
    }
}
