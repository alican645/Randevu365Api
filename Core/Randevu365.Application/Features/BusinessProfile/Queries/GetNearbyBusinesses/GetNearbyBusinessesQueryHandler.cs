using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetNearbyBusinesses;

public class GetNearbyBusinessesQueryHandler : IRequestHandler<GetNearbyBusinessesQueryRequest, ApiResponse<GetNearbyBusinessesQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNearbyBusinessesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetNearbyBusinessesQueryResponse>> Handle(GetNearbyBusinessesQueryRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetNearbyBusinessesQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<GetNearbyBusinessesQueryResponse>.FailResult(errors);
        }

        var businesses = await _unitOfWork.GetReadRepository<Business>()
            .GetAllAsync(
                predicate: x => !x.IsDeleted && x.BusinessLocations.Any(),
                include: i => i
                    .Include(b => b.BusinessLocations)
                    .Include(b => b.BusinessLogo)
            );

        var searchLat = (double)request.Latitude;
        var searchLon = (double)request.Longitude;

        var nearbyBusinesses = businesses
            .Select(b =>
            {
                var loc = b.BusinessLocations.First();
                var distance = CalculateDistanceKm(searchLat, searchLon, (double)loc.Latitude, (double)loc.Longitude);
                return new { Business = b, Location = loc, Distance = distance };
            })
            .Where(x => x.Distance <= request.RadiusKm)
            .OrderBy(x => x.Distance)
            .Select(x => new NearbyBusinessDto
            {
                BusinessId = x.Business.Id,
                BusinessName = x.Business.BusinessName,
                BusinessAddress = x.Business.BusinessAddress,
                BusinessCity = x.Business.BusinessCity,
                BusinessPhone = x.Business.BusinessPhone,
                BusinessLogo = x.Business.BusinessLogo?.LogoUrl,
                BusinessCategory = x.Business.BusinessCategory?.ToJson(),
                Location = new NearbyBusinessLocationDto
                {
                    Latitude = x.Location.Latitude,
                    Longitude = x.Location.Longitude
                },
                DistanceKm = Math.Round(x.Distance, 2)
            })
            .ToList();

        return ApiResponse<GetNearbyBusinessesQueryResponse>.SuccessResult(new GetNearbyBusinessesQueryResponse
        {
            Businesses = nearbyBusinesses
        });
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
