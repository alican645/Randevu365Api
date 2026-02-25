using Randevu365.Application.DTOs;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByCustomerOwnerId;

public class GetBusinessDetailInfoByCustomerOwnerIdQueryResponse
{
    public required string BusinessName { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessPhone { get; set; }
    public required string BusinessEmail { get; set; }
    public required string BusinessCountry { get; set; }
    public string? BusinessLogo { get; set; }
    public List<BusinessServiceDetailDto> BusinessServices { get; set; } = new();
    public List<BusinessHourDetailDto> BusinessHours { get; set; } = new();
    public List<BusinessPhotoDto> BusinessPhotos { get; set; } = new();
}




