namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessesByBusinessCategory;

public class GetBusinessesByBusinessCategoryResponse
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string BusinessCategory { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}