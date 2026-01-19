using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Commands.CreateBusiness;

public class CreateBusinessCommandRequest : IRequest<ApiResponse<CreateBusinessCommandResponse>>
{
    public string? BusinessName { get; set; }
    public string? BusinessAddress { get; set; }
    public string? BusinessCity { get; set; }
    public string? BusinessPhone { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessCountry { get; set; }
}
