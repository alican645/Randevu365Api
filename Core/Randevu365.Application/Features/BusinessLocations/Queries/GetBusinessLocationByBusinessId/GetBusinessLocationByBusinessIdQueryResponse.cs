using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLocations.Queries.GetBusinessLocationByBusinessId;

public class GetBusinessLocationByBusinessIdQueryResponse
{
    public required BusinessLocation BusinessLocation { get; set; }
}
