using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileByBusinessOwnerId;

public class GetBusinessProfileByBusinessOwnerIdQueryResponse
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

    public required List<string> BusinessServices { get; set; }
    public required List<string> BusinessHours { get; set; }
    public required List<string> BusinessPhotos { get; set; }
    public required List<string> BusinessComments { get; set; }
    public required decimal BusinessRatings { get; set; }
}
