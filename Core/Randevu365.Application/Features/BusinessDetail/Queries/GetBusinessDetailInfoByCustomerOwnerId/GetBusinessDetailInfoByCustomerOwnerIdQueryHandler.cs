using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.DTOs;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByCustomerOwnerId;

public class GetBusinessDetailInfoByCustomerOwnerIdQueryHandler : IRequestHandler<GetBusinessDetailInfoByCustomerOwnerIdQueryRequest, ApiResponse<GetBusinessDetailInfoByCustomerOwnerIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBusinessDetailInfoByCustomerOwnerIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetBusinessDetailInfoByCustomerOwnerIdQueryResponse>> Handle(GetBusinessDetailInfoByCustomerOwnerIdQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            return ApiResponse<GetBusinessDetailInfoByCustomerOwnerIdQueryResponse>.UnauthorizedResult();
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(
                predicate: x => x.AppUserId == currentUserId,
                include: i => i
                    .Include(b => b.BusinessLogo)
                    .Include(b => b.BusinessHours)
                    .Include(b => b.BusinessPhotos)
                    .Include(b => b.BusinessServices)
            );

        if (business == null)
        {
            return ApiResponse<GetBusinessDetailInfoByCustomerOwnerIdQueryResponse>.NotFoundResult("Kullanıcıya ait işletme bulunamadı.");
        }

        var response = new GetBusinessDetailInfoByCustomerOwnerIdQueryResponse
        {
            BusinessName = business.BusinessName,
            BusinessAddress = business.BusinessAddress,
            BusinessCity = business.BusinessCity,
            BusinessPhone = business.BusinessPhone,
            BusinessEmail = business.BusinessEmail,
            BusinessCountry = business.BusinessCountry,
            BusinessLogo = business.BusinessLogo?.LogoUrl,
            BusinessServices = business.BusinessServices?.Select(s => new BusinessServiceDetailDto
            {
                Id = s.Id,
                ServicePrice = s.ServicePrice,
                ServiceTitle = s.ServiceTitle,
                ServiceContent = s.ServiceContent,
                MaxConcurrentCustomers = s.MaxConcurrentCustomers
            }).ToList() ?? new List<BusinessServiceDetailDto>(),
            BusinessHours = business.BusinessHours?.Select(h => new BusinessHourDetailDto
            {
                Day = h.Day,
                OpenTime = h.OpenTime,
                CloseTime = h.CloseTime
            }).ToList() ?? new List<BusinessHourDetailDto>(),
            BusinessPhotos = business.BusinessPhotos?
                .Where(p => p.IsActive)
                .Select(p => new BusinessPhotoDto
                {
                    Id = p.Id,
                    PhotoPath = p.PhotoPath ?? string.Empty
                })
                .ToList() ?? new List<BusinessPhotoDto>()
        };

        return ApiResponse<GetBusinessDetailInfoByCustomerOwnerIdQueryResponse>.SuccessResult(response);
    }
}
