using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Commands.CreateBusiness;

public class CreateBusinessCommandRequest : IRequest<ApiResponse<CreateBusinessCommandResponse>>
{
    public required string BusinessName { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessPhone { get; set; }
    public required string BusinessEmail { get; set; }
    public required string BusinessCountry { get; set; }
    public int AppUserId { get; set; }
}
