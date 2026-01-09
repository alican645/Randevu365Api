using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByBusinessId;

public class GetCommentsByBusinessIdQueryRequest : IRequest<ApiResponse<IList<GetCommentsByBusinessIdQueryResponse>>>
{
    public int BusinessId { get; set; }
}
