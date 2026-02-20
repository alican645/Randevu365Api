namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessOwnerDashboard;

public class GetBusinessOwnerDashboardQueryResponse
{
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerSurname { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string OwnerPhone { get; set; } = string.Empty;

    public List<BusinessDashboardItemDto> Businesses { get; set; } = new();
}

public class BusinessDashboardItemDto
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessCity { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal AverageRating { get; set; }
    public int TodayAppointmentCount { get; set; }
    public string? LogoUrl { get; set; }
    public string? FirstPhotoPath { get; set; }
}
