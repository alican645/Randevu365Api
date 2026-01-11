using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessHours.Commands.UpdateBusinessHour;

public class UpdateBusinessHourCommandRequest : IRequest<ApiResponse<UpdateBusinessHourCommandResponse>>
{
    public int Id { get; set; }
    public required string Day { get; set; }
    public required string OpenTime { get; set; }
    public required string CloseTime { get; set; }
}
