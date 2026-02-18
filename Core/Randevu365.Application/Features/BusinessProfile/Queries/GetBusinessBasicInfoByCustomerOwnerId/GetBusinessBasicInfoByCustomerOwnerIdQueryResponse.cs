namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessBasicInfoByCustomerOwnerId;

public class GetBusinessBasicInfoByCustomerOwnerIdQueryResponse
{
    public required string BusinessName { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessPhone { get; set; }
    public required string BusinessEmail { get; set; }
    public required string BusinessCountry { get; set; }
    public required List<string> BusinessPhotos { get; set; }
}
