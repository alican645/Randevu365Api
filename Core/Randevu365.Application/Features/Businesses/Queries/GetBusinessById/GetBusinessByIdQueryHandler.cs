using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessById;

public class GetBusinessByIdQueryHandler : IRequestHandler<GetBusinessByIdQueryRequest, ApiResponse<GetBusinessByIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetBusinessByIdQueryResponse>> Handle(GetBusinessByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(
            predicate: x => x.Id == request.Id,
            include: q => q.Include(b => b.BusinessLocations).Include(b => b.BusinessPhotos)
        );

        if (business == null)
        {
            return ApiResponse<GetBusinessByIdQueryResponse>.NotFoundResult("Business not found.");
        }

        return ApiResponse<GetBusinessByIdQueryResponse>.SuccessResult(new GetBusinessByIdQueryResponse { Business = business });
    }
}
