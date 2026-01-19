using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileByUserId;

public class GetBusinessProfileByUserIdQueryHandler : IRequestHandler<GetBusinessProfileByUserIdQueryRequest, ApiResponse<GetBusinessProfileByUserIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessProfileByUserIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetBusinessProfileByUserIdQueryResponse>> Handle(GetBusinessProfileByUserIdQueryRequest request, CancellationToken cancellationToken)
    {
        // Validation
        var validator = new GetBusinessProfileByUserIdQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<GetBusinessProfileByUserIdQueryResponse>.FailResult(errors);
        }

        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(
            predicate: x => x.AppUserId == request.UserId,
            include: q => q.Include(b => b.BusinessLocations)
                           .Include(b => b.BusinessPhotos)
                           .Include(b => b.BusinessRatings)
                           .Include(b => b.BusinessLogo)
                           .Include(b => b.BusinessHours)
                           .Include(b => b.AppUser)
                           .ThenInclude(u => u.AppUserInformation)
        );

        if (business == null)
        {
            return ApiResponse<GetBusinessProfileByUserIdQueryResponse>.NotFoundResult("Business profile not found for this user.");
        }

        var response = new GetBusinessProfileByUserIdQueryResponse
        {
            BusinessName = business.BusinessName,
            BusinessAddress = business.BusinessAddress,
            BusinessCity = business.BusinessCity,
            BusinessPhone = business.BusinessPhone,
            BusinessEmail = business.BusinessEmail,
            BusinessCountry = business.BusinessCountry,
            BusinessLogo = business.BusinessLogo?.LogoUrl ?? string.Empty,

            BusinessOwnerName = business.AppUser?.AppUserInformation?.Name ?? string.Empty,
            BusinessOwnerPhone = business.AppUser?.AppUserInformation?.PhoneNumber ?? string.Empty,
            BusinessOwnerEmail = business.AppUser?.Email ?? string.Empty,
            // Assuming owner address/city/country might not be in AppUserInformation or need to be mapped.
            // Based on available fields, I'll use existing data or placeholders if not available.
            // AppUserInformation usually holds personal info.
            BusinessOwnerAddress = string.Empty, // Not directly available in AppUserInformation shown previously
            BusinessOwnerCity = string.Empty,    // Not directly available
            BusinessOwnerCountry = string.Empty, // Not directly available

            BusinessServices = new List<string>(), // Placeholder as BusinessServices entity is missing
            BusinessHours = business.BusinessHours.Select(h => $"{h.Day}: {h.OpenTime} - {h.CloseTime}").ToList(),
            BusinessPhotos = business.BusinessPhotos.Where(p => p.PhotoPath != null).Select(p => p.PhotoPath!).ToList(),
            BusinessComments = business.BusinessComments.Select(c => c.Comment).ToList(),
            BusinessRatings = business.BusinessRatings.Any() ? (decimal)business.BusinessRatings.Average(r => r.Rating) : 0
        };

        return ApiResponse<GetBusinessProfileByUserIdQueryResponse>.SuccessResult(response);
    }
}
