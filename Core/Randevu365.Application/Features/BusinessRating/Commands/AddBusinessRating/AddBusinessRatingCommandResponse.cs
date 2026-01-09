namespace Randevu365.Application.Features.BusinessRating.Commands.AddBusinessRating;

public class AddBusinessRatingCommandResponse
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int AppUserId { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
