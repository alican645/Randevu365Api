using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessComments.Commands.AddBusinessComment;

public class AddBusinessCommentCommandRequest : IRequest<ApiResponse<AddBusinessCommentCommandResponse>>
{
    public int BusinessId { get; set; }
    public required string Comment { get; set; }
    // AppUserId JWT'den alınacak
}
