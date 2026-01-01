using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessLocations.Commands.UpdateBusinessLocation;

public class UpdateBusinessLocationCommandRequest : IRequest<ApiResponse<UpdateBusinessLocationCommandResponse>>
{
    public int Id { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
