namespace Randevu365.Application.Features.BusinessProfile.Queries.GetNearbyBusinesses;

public class GetNearbyBusinessesQueryResponse
{
    public List<NearbyBusinessDto> Businesses { get; set; } = new();
}

public class NearbyBusinessDto
{
    public int BusinessId { get; set; }
    public required string BusinessName { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessPhone { get; set; }
    public string? BusinessLogo { get; set; }
    public string? BusinessCategory { get; set; }
    public NearbyBusinessLocationDto Location { get; set; } = null!;
    public double DistanceKm { get; set; }
}

public class NearbyBusinessLocationDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
