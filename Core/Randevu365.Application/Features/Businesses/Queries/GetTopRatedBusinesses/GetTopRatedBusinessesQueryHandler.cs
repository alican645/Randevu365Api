using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Queries.GetTopRatedBusinesses;

public class GetTopRatedBusinessesQueryHandler : IRequestHandler<GetTopRatedBusinessesQueryRequest, ApiResponse<GetTopRatedBusinessesQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTopRatedBusinessesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetTopRatedBusinessesQueryResponse>> Handle(GetTopRatedBusinessesQueryRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetTopRatedBusinessesQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<GetTopRatedBusinessesQueryResponse>.FailResult(
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var businesses = await _unitOfWork.GetReadRepository<Business>().GetAllAsync(
            include: q => q.Include(b => b.BusinessLogo)
                          .Include(b => b.BusinessRatings)
                          .Include(b => b.BusinessComments),
            orderBy: q => q.OrderByDescending(b => b.BusinessRatings.Any() ? b.BusinessRatings.Average(r => r.Rating) : 0)
        );

        var responseItems = businesses.Take(request.Count).Select(b => new GetTopRatedBusinessesQueryResponseItem
        {
            Id = b.Id,
            BusinessName = b.BusinessName,
            BusinessAddress = b.BusinessAddress,
            BusinessCategory = b.BusinessCategory?.ToJson() ?? string.Empty,
            LogoUrl = b.BusinessLogo?.LogoUrl,
            AverageRating = b.BusinessRatings.Any() ? Math.Round(b.BusinessRatings.Average(r => r.Rating), 1) : 0,
            TotalCommentCount = b.BusinessComments.Count
        }).ToList();

        return ApiResponse<GetTopRatedBusinessesQueryResponse>.SuccessResult(new GetTopRatedBusinessesQueryResponse
        {
            Businesses = responseItems
        });
    }
}
