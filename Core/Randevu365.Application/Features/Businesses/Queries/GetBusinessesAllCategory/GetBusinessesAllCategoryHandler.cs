using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessesAllCategory;

public class GetBusinessesAllCategoryHandler : IRequestHandler<GetBusinessesAllCategoryRequest, ApiResponse<PaginatedListDto<GetBusinessesAllCategoryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessesAllCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PaginatedListDto<GetBusinessesAllCategoryResponse>>> Handle(GetBusinessesAllCategoryRequest request, CancellationToken cancellationToken)
    {
        var totalCount = await _unitOfWork.GetReadRepository<Business>().CountAsync();

        var businesses = await _unitOfWork.GetReadRepository<Business>().GetAllByPagingAsync(
            include: q => q.Include(b => b.BusinessLogo),
            currentPage: request.PageNumber,
            pageSize: request.PageSize
        );

        var responseItems = businesses.Select(b => new GetBusinessesAllCategoryResponse
        {
            Id = b.Id,
            BusinessName = b.BusinessName,
            BusinessAddress = b.BusinessAddress,
            BusinessCategory = b.BusinessCategory?.ToJson() ?? string.Empty,
            LogoUrl = b.BusinessLogo?.LogoUrl
        }).ToList();

        var paginatedResult = new PaginatedListDto<GetBusinessesAllCategoryResponse>(
            responseItems,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return ApiResponse<PaginatedListDto<GetBusinessesAllCategoryResponse>>.SuccessResult(paginatedResult);
    }
}
