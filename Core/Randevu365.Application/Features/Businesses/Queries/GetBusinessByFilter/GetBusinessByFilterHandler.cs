using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessByFilter;

public class GetBusinessByFilterHandler : IRequestHandler<GetBusinessByFilterRequest, ApiResponse<PaginatedListDto<GetBusinessByFilterResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessByFilterHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PaginatedListDto<GetBusinessByFilterResponse>>> Handle(GetBusinessByFilterRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetBusinessByFilterValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApiResponse<PaginatedListDto<GetBusinessByFilterResponse>>.FailResult(
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        // Parse category if provided
        BusinessCategory? parsedCategory = null;
        if (!string.IsNullOrEmpty(request.Category))
        {
            if (!BusinessCategoryExtensions.TryFromJson(request.Category, out var category))
            {
                return ApiResponse<PaginatedListDto<GetBusinessByFilterResponse>>.FailResult("Geçersiz kategori adı.");
            }
            parsedCategory = category;
        }

        // Build predicate
        Expression<Func<Business, bool>>? predicate = null;

        if (parsedCategory != null && !string.IsNullOrEmpty(request.QueryParam))
        {
            var normalizedQuery = NormalizeTurkish(request.QueryParam);
            var cat = parsedCategory.Value;
            predicate = b => b.BusinessCategory == cat &&
                             b.BusinessName.ToLower()
                                 .Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                                 .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c")
                                 .Contains(normalizedQuery);
        }
        else if (parsedCategory != null)
        {
            var cat = parsedCategory.Value;
            predicate = b => b.BusinessCategory == cat;
        }
        else if (!string.IsNullOrEmpty(request.QueryParam))
        {
            var normalizedQuery = NormalizeTurkish(request.QueryParam);
            predicate = b => b.BusinessName.ToLower()
                                 .Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                                 .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c")
                                 .Contains(normalizedQuery);
        }

        // Build orderBy
        Func<IQueryable<Business>, IOrderedQueryable<Business>>? orderBy = null;

        if (request.OrderByRating == true)
        {
            orderBy = q => q.OrderByDescending(b => b.BusinessRatings.Any() ? b.BusinessRatings.Average(r => r.Rating) : 0);
        }
        else if (request.OrderByCommentCount == true)
        {
            orderBy = q => q.OrderByDescending(b => b.BusinessComments.Count);
        }
        else
        {
            orderBy = q => q.OrderBy(b => b.Id);
        }

        var totalCount = await _unitOfWork.GetReadRepository<Business>().CountAsync(predicate);

        var businesses = await _unitOfWork.GetReadRepository<Business>().GetAllByPagingAsync(
            predicate: predicate,
            include: q => q.Include(b => b.BusinessLogo)
                          .Include(b => b.BusinessRatings)
                          .Include(b => b.BusinessComments),
            orderBy: orderBy,
            currentPage: request.PageNumber,
            pageSize: request.PageSize
        );

        var responseItems = businesses.Select(b => new GetBusinessByFilterResponse
        {
            Id = b.Id,
            BusinessName = b.BusinessName,
            BusinessAddress = b.BusinessAddress,
            BusinessCategory = b.BusinessCategory?.ToJson() ?? string.Empty,
            LogoUrl = b.BusinessLogo?.LogoUrl,
            AverageRating = b.BusinessRatings.Any() ? Math.Round(b.BusinessRatings.Average(r => r.Rating), 1) : 0,
            TotalCommentCount = b.BusinessComments.Count
        }).ToList();

        var paginatedResult = new PaginatedListDto<GetBusinessByFilterResponse>(
            responseItems,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return ApiResponse<PaginatedListDto<GetBusinessByFilterResponse>>.SuccessResult(paginatedResult);
    }

    private static string NormalizeTurkish(string input)
    {
        return input.ToLower()
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c");
    }
}
