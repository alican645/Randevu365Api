using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessComments.Commands.UpdateBusinessComment;

public class UpdateBusinessCommentCommandRequest : IRequest<ApiResponse<UpdateBusinessCommentCommandResponse>>
{
    public int CommentId { get; set; }
    public required string Comment { get; set; }
    // AppUserId JWT'den alınarak yetki kontrolü yapılacak
}
