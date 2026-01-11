namespace Randevu365.Application.Features.BusinessHours.Queries.GetBusinessHoursByBusinessId;

public class GetBusinessHoursByBusinessIdQueryResponse
{
    public int Id { get; set; }
    public required string Day { get; set; }
    public required string OpenTime { get; set; }
    public required string CloseTime { get; set; }
}
