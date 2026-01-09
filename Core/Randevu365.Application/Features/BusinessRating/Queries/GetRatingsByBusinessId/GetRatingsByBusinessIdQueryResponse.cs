namespace Randevu365.Application.Features.BusinessRating.Queries.GetRatingsByBusinessId;

public class GetRatingsByBusinessIdQueryResponse
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int AppUserId { get; set; }
    public int Rating { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}
