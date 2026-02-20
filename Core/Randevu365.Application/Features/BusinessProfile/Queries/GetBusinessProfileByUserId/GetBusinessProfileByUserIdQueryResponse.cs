namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileByUserId;

public class GetBusinessProfileByUserIdQueryResponse
{
    public required string BusinessName { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessPhone { get; set; }
    public required string BusinessEmail { get; set; }
    public required string BusinessCountry { get; set; }

    public required string BusinessLogo { get; set; }
    public required string BusinessOwnerName { get; set; }
    public required string BusinessOwnerPhone { get; set; }
    public required string BusinessOwnerEmail { get; set; }
    public required string BusinessOwnerAddress { get; set; }
    public required string BusinessOwnerCity { get; set; }
    public required string BusinessOwnerCountry { get; set; }

    public required List<BusinessServiceDto> BusinessServices { get; set; }
    public required List<string> BusinessHours { get; set; }
    public required List<string> BusinessPhotos { get; set; }
    public required List<string> BusinessComments { get; set; }
    public required decimal BusinessRatings { get; set; }
}

public class BusinessServiceDto
{
    public required string ServiceTitle { get; set; }
    public required string ServiceContent { get; set; }
}
