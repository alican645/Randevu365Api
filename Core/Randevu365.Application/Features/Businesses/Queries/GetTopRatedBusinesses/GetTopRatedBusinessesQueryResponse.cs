namespace Randevu365.Application.Features.Businesses.Queries.GetTopRatedBusinesses;

public class GetTopRatedBusinessesQueryResponseItem
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string BusinessCategory { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public double AverageRating { get; set; }
    public int TotalCommentCount { get; set; }
}


public class GetTopRatedBusinessesQueryResponse
{
    public List<GetTopRatedBusinessesQueryResponseItem> Businesses { get; set; }
}