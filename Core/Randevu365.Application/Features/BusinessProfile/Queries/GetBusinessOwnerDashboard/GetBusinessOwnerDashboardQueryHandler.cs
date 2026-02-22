using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;
using AppUserEntity = Randevu365.Domain.Entities.AppUser;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessOwnerDashboard;

public class GetBusinessOwnerDashboardQueryHandler : IRequestHandler<GetBusinessOwnerDashboardQueryRequest, ApiResponse<GetBusinessOwnerDashboardQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBusinessOwnerDashboardQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetBusinessOwnerDashboardQueryResponse>> Handle(GetBusinessOwnerDashboardQueryRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            return ApiResponse<GetBusinessOwnerDashboardQueryResponse>.UnauthorizedResult();
        }

        var appUser = await _unitOfWork.GetReadRepository<AppUserEntity>()
            .GetAsync(
                predicate: x => x.Id == currentUserId,
                include: q => q.Include(u => u.AppUserInformation)
            );

        if (appUser == null)
        {
            return ApiResponse<GetBusinessOwnerDashboardQueryResponse>.UnauthorizedResult();
        }
        
        var slots = await _unitOfWork.GetReadRepository<BusinessSlot>()
            .GetAllAsync(predicate: x => x.AppUserId == currentUserId
                                      && !x.IsDeleted
                                      && !x.IsUsed
                                      && x.PaymentStatus == SlotPaymentStatus.Completed);


        var businesses = await _unitOfWork.GetReadRepository<Business>()
            .GetAllAsync(
                predicate: x => x.AppUserId == currentUserId.Value && !x.IsDeleted,
                include: q => q
                    .Include(b => b.BusinessLocations)
                    .Include(b => b.BusinessRatings)
                    .Include(b => b.BusinessLogo)
                    .Include(b => b.BusinessPhotos)
                    .Include(b => b.Appointments)
            );

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var businessItems = businesses.Select(business => new BusinessDashboardItemDto
        {
            Id = business.Id,
            BusinessName = business.BusinessName,
            BusinessCity = business.BusinessCity,
            Latitude = business.BusinessLocations.FirstOrDefault()?.Latitude,
            Longitude = business.BusinessLocations.FirstOrDefault()?.Longitude,
            AverageRating = business.BusinessRatings?.Any() == true
                ? (decimal)business.BusinessRatings.Average(r => r.Rating)
                : 0,
            TodayAppointmentCount = business.Appointments.Count(a =>
                a.AppointmentDate == today &&
                !a.IsDeleted &&
                (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed)),
            LogoUrl = business.BusinessLogo?.LogoUrl,
            FirstPhotoPath = business.BusinessPhotos?
                .Where(p => p.IsActive)
                .OrderBy(p => p.Id)
                .FirstOrDefault()?.PhotoPath
        }).ToList();

        var response = new GetBusinessOwnerDashboardQueryResponse
        {
            OwnerName = appUser.AppUserInformation?.Name ?? string.Empty,
            OwnerSurname = appUser.AppUserInformation?.Surname ?? string.Empty,
            OwnerEmail = appUser.Email,
            OwnerPhone = appUser.AppUserInformation?.PhoneNumber ?? string.Empty,
            Businesses = businessItems,
            BusinessSlotCount = slots.Count
        };

        return ApiResponse<GetBusinessOwnerDashboardQueryResponse>.SuccessResult(response);
    }
}
