namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessByFilter;

public class GetBusinessByFilterResponse
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string BusinessCategory { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public double AverageRating { get; set; }
    public int TotalCommentCount { get; set; }
}
