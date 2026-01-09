namespace Randevu365.Application.Features.Businesses.Queries.GetAllBusinesses;

public class GetAllBusinessesQueryResponse
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string BusinessCity { get; set; } = string.Empty;
    public string BusinessPhone { get; set; } = string.Empty;
    public string BusinessEmail { get; set; } = string.Empty;
    public string BusinessCountry { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int AppUserId { get; set; }
    public string? OwnerName { get; set; }
    public IList<BusinessLocationDto> BusinessLocations { get; set; } = new List<BusinessLocationDto>();
}

public class BusinessLocationDto
{
    public int Id { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
