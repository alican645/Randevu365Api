using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.DTOs;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByBusinessId;

public class GetBusinessDetailInfoByBusinessIdQueryHandler : IRequestHandler<GetBusinessDetailInfoByBusinessIdQueryRequest, ApiResponse<GetBusinessDetailInfoByBusinessIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessDetailInfoByBusinessIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetBusinessDetailInfoByBusinessIdQueryResponse>> Handle(GetBusinessDetailInfoByBusinessIdQueryRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetBusinessDetailInfoByBusinessIdQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<GetBusinessDetailInfoByBusinessIdQueryResponse>.FailResult(errors);
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(
                predicate: x => x.Id == request.BusinessId && !x.IsDeleted,
                include: i => i
                    .Include(b => b.BusinessLogo)
                    .Include(b => b.BusinessHours)
                    .Include(b => b.BusinessPhotos)
                    .Include(b => b.BusinessServices)
                    .Include(b => b.BusinessLocations)
            );

        if (business == null)
        {
            return ApiResponse<GetBusinessDetailInfoByBusinessIdQueryResponse>.NotFoundResult("İşletme bulunamadı.");
        }

        var response = new GetBusinessDetailInfoByBusinessIdQueryResponse
        {
            BusinessName = business.BusinessName,
            BusinessAddress = business.BusinessAddress,
            BusinessCity = business.BusinessCity,
            BusinessPhone = business.BusinessPhone,
            BusinessEmail = business.BusinessEmail,
            BusinessCountry = business.BusinessCountry,
            BusinessLogo = business.BusinessLogo?.LogoUrl,
            BusinessCategory = business.BusinessCategory?.ToJson(),
            BusinessServices = business.BusinessServices?.Select(s => new BusinessServiceDetailDto
            {
                Id =  s.Id,
                ServicePrice =  s.ServicePrice,
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
                .ToList() ?? new List<BusinessPhotoDto>(),
            Location = business.BusinessLocations.FirstOrDefault() is { } loc
                ? new BusinessLocationDto { Latitude = loc.Latitude, Longitude = loc.Longitude }
                : null
        };

        return ApiResponse<GetBusinessDetailInfoByBusinessIdQueryResponse>.SuccessResult(response);
    }
}
