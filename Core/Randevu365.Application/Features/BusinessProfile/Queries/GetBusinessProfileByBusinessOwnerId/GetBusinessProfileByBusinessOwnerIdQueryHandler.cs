using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileByBusinessOwnerId;

public class GetBusinessProfileByBusinessOwnerIdQueryHandler : IRequestHandler<GetBusinessProfileByBusinessOwnerIdQueryRequest, ApiResponse<GetBusinessProfileByBusinessOwnerIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBusinessProfileByBusinessOwnerIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetBusinessProfileByBusinessOwnerIdQueryResponse>> Handle(GetBusinessProfileByBusinessOwnerIdQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            return ApiResponse<GetBusinessProfileByBusinessOwnerIdQueryResponse>.UnauthorizedResult();
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(
                predicate: x => x.AppUserId == currentUserId,
                include: i => i
                    .Include(b => b.AppUser).ThenInclude(u => u.AppUserInformation)
                    .Include(b => b.BusinessLocations)
                    .Include(b => b.BusinessHours)
                    .Include(b => b.BusinessLogo)
                    .Include(b => b.BusinessPhotos)
                    .Include(b => b.BusinessComments)
                    .Include(b => b.BusinessRatings)
            );

        if (business == null)
        {
            return ApiResponse<GetBusinessProfileByBusinessOwnerIdQueryResponse>.NotFoundResult("Kullanıcıya ait işletme profili bulunamadı.");
        }

        var response = new GetBusinessProfileByBusinessOwnerIdQueryResponse
        {
            BusinessName = business.BusinessName,
            BusinessAddress = business.BusinessAddress,
            BusinessCity = business.BusinessCity,
            BusinessPhone = business.BusinessPhone,
            BusinessEmail = business.BusinessEmail,
            BusinessCountry = business.BusinessCountry,

            BusinessLogo = business.BusinessLogo?.LogoUrl,

            BusinessOwnerName = business.AppUser?.AppUserInformation != null
                ? $"{business.AppUser.AppUserInformation.Name} {business.AppUser.AppUserInformation.Surname}"
                : string.Empty,
            BusinessOwnerPhone = business.AppUser?.AppUserInformation?.PhoneNumber ?? string.Empty,
            BusinessOwnerEmail = business.AppUser?.Email ?? string.Empty,
            BusinessOwnerAddress = string.Empty, // Add if available in AppUserInformation
            BusinessOwnerCity = string.Empty,    // Add if available in AppUserInformation
            BusinessOwnerCountry = string.Empty, // Add if available in AppUserInformation

            BusinessServices = new List<string>(), // Implement when services are added
            BusinessHours = business.BusinessHours?.Select(h => $"{h.Day}: {h.OpenTime}-{h.CloseTime}").ToList() ?? new List<string>(),
            BusinessPhotos = business.BusinessPhotos?.Where(p => p.IsActive).Select(p => p.PhotoPath).ToList() ?? new List<string>(),
            BusinessComments = business.BusinessComments?.Select(c => c.Comment).ToList() ?? new List<string>(),
            BusinessRatings = business.BusinessRatings?.Any() == true
                ? (decimal)business.BusinessRatings.Average(r => r.Rating)
                : 0
        };

        return ApiResponse<GetBusinessProfileByBusinessOwnerIdQueryResponse>.SuccessResult(response);
    }
}
