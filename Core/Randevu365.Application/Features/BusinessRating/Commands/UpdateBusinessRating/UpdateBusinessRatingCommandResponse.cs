namespace Randevu365.Application.Features.BusinessRating.Commands.UpdateBusinessRating;

public class UpdateBusinessRatingCommandResponse
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int AppUserId { get; set; }
    public int Rating { get; set; }
}
