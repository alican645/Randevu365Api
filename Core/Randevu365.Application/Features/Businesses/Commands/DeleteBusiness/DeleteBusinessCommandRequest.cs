using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Businesses.Commands.DeleteBusiness;

public class DeleteBusinessCommandRequest : IRequest<ApiResponse<DeleteBusinessCommandResponse>>
{
    public int Id { get; set; }
}
