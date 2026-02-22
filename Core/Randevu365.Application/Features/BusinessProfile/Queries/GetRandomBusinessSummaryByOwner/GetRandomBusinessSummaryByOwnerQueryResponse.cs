namespace Randevu365.Application.Features.BusinessProfile.Queries.GetRandomBusinessSummaryByOwner;

public class GetRandomBusinessSummaryByOwnerQueryResponse
{
    public int BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public int CommentCount { get; set; }
    public string? LogoUrl { get; set; }
    public string? FirstPhotoPath { get; set; }
    public int AppointmentCount { get; set; }
    
}
