using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessLocations.Commands.CreateBusinessLocation;

public class CreateBusinessLocationCommandRequest : IRequest<ApiResponse<CreateBusinessLocationCommandResponse>>
{
    public int BusinessId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
