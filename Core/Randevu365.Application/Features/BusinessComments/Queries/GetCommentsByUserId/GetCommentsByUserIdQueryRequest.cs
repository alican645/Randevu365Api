using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByUserId;

public class GetCommentsByUserIdQueryRequest : IRequest<ApiResponse<IList<GetCommentsByUserIdQueryResponse>>>
{
    // UserId JWT'den alınacak, request'e eklenmeyecek
}
