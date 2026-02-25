using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetCustomerBusinessProfile;

public class GetCustomerBusinessProfileQueryHandler
    : IRequestHandler<GetCustomerBusinessProfileQueryRequest, ApiResponse<GetCustomerBusinessProfileQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerBusinessProfileQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetCustomerBusinessProfileQueryResponse>> Handle(
        GetCustomerBusinessProfileQueryRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new GetCustomerBusinessProfileQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<GetCustomerBusinessProfileQueryResponse>.FailResult(errors);
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
                    .Include(b => b.BusinessRatings)
                    .Include(b => b.BusinessComments)
            );

        if (business == null)
            return ApiResponse<GetCustomerBusinessProfileQueryResponse>.NotFoundResult("İşletme bulunamadı.");

        var activeRatings = business.BusinessRatings.Where(r => !r.IsDeleted).ToList();
        var averageRating = activeRatings.Any()
            ? Math.Round((decimal)activeRatings.Average(r => r.Rating), 1)
            : 0m;

        var commentCount = business.BusinessComments.Count(c => !c.IsDeleted);

        var response = new GetCustomerBusinessProfileQueryResponse
        {
            BusinessId = business.Id,
            BusinessName = business.BusinessName,
            BusinessAddress = business.BusinessAddress,
            BusinessCity = business.BusinessCity,
            BusinessPhone = business.BusinessPhone,
            BusinessEmail = business.BusinessEmail,
            BusinessCategory = business.BusinessCategory?.ToJson(),
            LogoUrl = business.BusinessLogo?.LogoUrl,

            Location = business.BusinessLocations.FirstOrDefault() is { } loc
                ? new CustomerBusinessLocationDto { Latitude = loc.Latitude, Longitude = loc.Longitude }
                : null,

            Services = business.BusinessServices?
                .Where(s => !s.IsDeleted)
                .Select(s => new CustomerBusinessServiceDto
                {
                    Id = s.Id,
                    ServiceTitle = s.ServiceTitle,
                    ServiceContent = s.ServiceContent,
                    ServicePrice =  s.ServicePrice,
                    MaxConcurrentCustomers = s.MaxConcurrentCustomers
                }).ToList() ?? new(),

            BusinessHours = business.BusinessHours?
                .Where(h => !h.IsDeleted)
                .Select(h => new CustomerBusinessHourDto
                {
                    Day = h.Day,
                    OpenTime = h.OpenTime,
                    CloseTime = h.CloseTime
                }).ToList() ?? new(),

            Photos = business.BusinessPhotos?
                .Where(p => p.IsActive && !p.IsDeleted)
                .Select(p => new CustomerBusinessPhotoDto
                {
                    PhotoPath = p.PhotoPath ?? string.Empty
                }).ToList() ?? new(),

            AverageRating = averageRating,
            CommentCount = commentCount
        };

        return ApiResponse<GetCustomerBusinessProfileQueryResponse>.SuccessResult(response);
    }
}
