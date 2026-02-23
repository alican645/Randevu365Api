using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetNearbyBusinesses;

public class GetNearbyBusinessesQueryRequest : IRequest<ApiResponse<GetNearbyBusinessesQueryResponse>>
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public double RadiusKm { get; set; } = 20;
}
