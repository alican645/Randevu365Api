using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessComments.Commands.DeleteBusinessComment;

public class DeleteBusinessCommentCommandRequest : IRequest<ApiResponse<DeleteBusinessCommentCommandResponse>>
{
    public int CommentId { get; set; }
    // AppUserId JWT'den alınarak yetki kontrolü yapılacak
}
