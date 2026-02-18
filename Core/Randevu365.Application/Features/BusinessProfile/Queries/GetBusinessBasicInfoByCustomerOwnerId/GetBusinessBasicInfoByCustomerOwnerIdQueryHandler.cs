using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessBasicInfoByCustomerOwnerId;

public class GetBusinessBasicInfoByCustomerOwnerIdQueryHandler : IRequestHandler<GetBusinessBasicInfoByCustomerOwnerIdQueryRequest, ApiResponse<GetBusinessBasicInfoByCustomerOwnerIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBusinessBasicInfoByCustomerOwnerIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetBusinessBasicInfoByCustomerOwnerIdQueryResponse>> Handle(GetBusinessBasicInfoByCustomerOwnerIdQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            return ApiResponse<GetBusinessBasicInfoByCustomerOwnerIdQueryResponse>.UnauthorizedResult();
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(
                predicate: x => x.AppUserId == currentUserId,
                include: i => i.Include(b => b.BusinessPhotos)
            );

        if (business == null)
        {
            return ApiResponse<GetBusinessBasicInfoByCustomerOwnerIdQueryResponse>.NotFoundResult("Kullanıcıya ait işletme bulunamadı.");
        }

        var response = new GetBusinessBasicInfoByCustomerOwnerIdQueryResponse
        {
            BusinessName = business.BusinessName,
            BusinessAddress = business.BusinessAddress,
            BusinessCity = business.BusinessCity,
            BusinessPhone = business.BusinessPhone,
            BusinessEmail = business.BusinessEmail,
            BusinessCountry = business.BusinessCountry,
            BusinessPhotos = business.BusinessPhotos?.Where(p => p.IsActive).Select(p => p.PhotoPath ?? string.Empty).ToList() ?? new List<string>()
        };

        return ApiResponse<GetBusinessBasicInfoByCustomerOwnerIdQueryResponse>.SuccessResult(response);
    }
}
