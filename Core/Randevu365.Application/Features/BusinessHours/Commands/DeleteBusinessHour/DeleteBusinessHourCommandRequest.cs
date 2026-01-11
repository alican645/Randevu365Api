using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessHours.Commands.DeleteBusinessHour;

public class DeleteBusinessHourCommandRequest : IRequest<ApiResponse<DeleteBusinessHourCommandResponse>>
{
    public int Id { get; set; }
}
