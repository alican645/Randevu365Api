using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessPhotos.Queries.GetBusinessPhotosByBusinessId;

public class GetBusinessPhotosByBusinessIdQueryResponse
{
    public required BusinessPhoto BusinessPhoto { get; set; }
}
