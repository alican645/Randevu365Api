using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessLocations.Commands.DeleteBusinessLocation;

public class DeleteBusinessLocationCommandRequest : IRequest<ApiResponse<DeleteBusinessLocationCommandResponse>>
{
    public int Id { get; set; }
}
