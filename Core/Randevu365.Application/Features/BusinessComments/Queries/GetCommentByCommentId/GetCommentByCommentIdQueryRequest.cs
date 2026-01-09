using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentByCommentId;

public class GetCommentByCommentIdQueryRequest : IRequest<ApiResponse<GetCommentByCommentIdQueryResponse>>
{
    public int CommentId { get; set; }
}
