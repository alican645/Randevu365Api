using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Commands.UpdateBusiness;

public class UpdateBusinessCommandRequest : IRequest<ApiResponse<UpdateBusinessCommandResponse>>
{
    public int? Id { get; set; }
    public string? BusinessName { get; set; }
    public string? BusinessAddress { get; set; }
    public string? BusinessCity { get; set; }
    public string? BusinessPhone { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessCountry { get; set; }
}
