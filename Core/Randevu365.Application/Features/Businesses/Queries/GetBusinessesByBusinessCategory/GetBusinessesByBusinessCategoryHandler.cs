using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessesByBusinessCategory;

public class GetBusinessesByBusinessCategoryHandler : IRequestHandler<GetBusinessesByBusinessCategoryRequest, ApiResponse<PaginatedListDto<GetBusinessesByBusinessCategoryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessesByBusinessCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PaginatedListDto<GetBusinessesByBusinessCategoryResponse>>> Handle(GetBusinessesByBusinessCategoryRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetBusinessesByBusinessCategoryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<PaginatedListDto<GetBusinessesByBusinessCategoryResponse>>.FailResult(
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        if (!BusinessCategoryExtensions.TryFromJson(request.CategoryName, out var category))
        {
            return ApiResponse<PaginatedListDto<GetBusinessesByBusinessCategoryResponse>>.FailResult("Geçersiz kategori adı.");
        }

        var totalCount = await _unitOfWork.GetReadRepository<Business>().CountAsync(b => b.BusinessCategory == category);

        var businesses = await _unitOfWork.GetReadRepository<Business>().GetAllByPagingAsync(
            predicate: b => b.BusinessCategory == category,
            include: q => q.Include(b => b.BusinessLogo)
                          .Include(b => b.BusinessRatings)
                          .Include(b => b.BusinessComments),
            currentPage: request.PageNumber,
            pageSize: request.PageSize
        );

        var responseItems = businesses.Select(b => new GetBusinessesByBusinessCategoryResponse
        {
            Id = b.Id,
            BusinessName = b.BusinessName,
            BusinessAddress = b.BusinessAddress,
            BusinessCategory = b.BusinessCategory?.ToJson() ?? string.Empty,
            LogoUrl = b.BusinessLogo?.LogoUrl,
            AverageRating = b.BusinessRatings.Any() ? Math.Round(b.BusinessRatings.Average(r => r.Rating), 1) : 0,
            TotalCommentCount = b.BusinessComments.Count
        }).ToList();

        var paginatedResult = new PaginatedListDto<GetBusinessesByBusinessCategoryResponse>(
            responseItems,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return ApiResponse<PaginatedListDto<GetBusinessesByBusinessCategoryResponse>>.SuccessResult(paginatedResult);
    }
}
