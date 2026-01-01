using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessPhotos.Queries.GetBusinessPhotosByBusinessId;

public class GetBusinessPhotosByBusinessIdQueryRequest : IRequest<ApiResponse<IList<GetBusinessPhotosByBusinessIdQueryResponse>>>
{
    public int BusinessId { get; set; }
}
