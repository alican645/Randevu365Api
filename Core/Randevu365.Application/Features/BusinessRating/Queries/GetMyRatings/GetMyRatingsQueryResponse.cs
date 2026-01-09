namespace Randevu365.Application.Features.BusinessRating.Queries.GetMyRatings;

public class GetMyRatingsQueryResponse
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public string? BusinessName { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
