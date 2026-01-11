using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessHours.Commands.CreateBusinessHour;

public class CreateBusinessHourCommandRequest : IRequest<ApiResponse<CreateBusinessHourCommandResponse>>
{
    public required string Day { get; set; }
    public required string OpenTime { get; set; }
    public required string CloseTime { get; set; }
    public int BusinessId { get; set; }
}
