using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Queries.GetAllBusinessesPaginated;

public class GetAllBusinessesPaginatedHandler : IRequestHandler<GetAllBusinessesPaginatedRequest, ApiResponse<PaginatedListDto<GetAllBusinessesPaginatedResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBusinessesPaginatedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PaginatedListDto<GetAllBusinessesPaginatedResponse>>> Handle(GetAllBusinessesPaginatedRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetAllBusinessesPaginatedValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ApiResponse<PaginatedListDto<GetAllBusinessesPaginatedResponse>>.FailResult(
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var totalCount = await _unitOfWork.GetReadRepository<Business>().CountAsync();

        var businesses = await _unitOfWork.GetReadRepository<Business>().GetAllByPagingAsync(
            include: q => q.Include(b => b.BusinessLogo),
            currentPage: request.PageNumber,
            pageSize: request.PageSize
        );

        var responseItems = businesses.Select(b => new GetAllBusinessesPaginatedResponse
        {
            Id = b.Id,
            BusinessName = b.BusinessName,
            BusinessAddress = b.BusinessAddress,
            BusinessCategory = b.BusinessCategory?.ToJson() ?? string.Empty,
            LogoUrl = b.BusinessLogo?.LogoUrl
        }).ToList();

        var paginatedResult = new PaginatedListDto<GetAllBusinessesPaginatedResponse>(
            responseItems,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return ApiResponse<PaginatedListDto<GetAllBusinessesPaginatedResponse>>.SuccessResult(paginatedResult);
    }
}
